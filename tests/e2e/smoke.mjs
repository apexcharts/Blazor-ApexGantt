// Headless browser smoke test for the Blazor-ApexGantt sample.
//
// Why this exists: a Blazor chart wrapper can build and even serialize its options perfectly yet
// still fail in the browser, because the real contract lives in the JS interop and the core library.
// This wrapper loads the core as an ES module and serializes ~90 options via System.Text.Json, so a
// broken import path, a mis-serialized option, or an events wiring regression would only surface at
// runtime. This test drives the actual WASM app in a real browser and fails CI if any demo page
// throws, shows the Blazor error UI, or renders no gantt chart.
//
// Usage:
//   BASE_URL=http://localhost:5185 node smoke.mjs
//   PW_CHANNEL=chrome node smoke.mjs   # drive an installed Chrome instead of bundled chromium
let chromium;
try {
  ({ chromium } = await import("playwright"));
} catch {
  ({ chromium } = await import("playwright-core"));
}

const BASE = process.env.BASE_URL || "http://localhost:5185";
const channel = process.env.PW_CHANNEL;

// Chart demo routes. Each must render a gantt chart (svg + task bars) with no console/page errors.
// The home route ("/") is a landing page and is exercised by the boot step below (errors only).
const ROUTES = [
  "gantt-demo",
  "styling-demo",
  "interactive-demo",
  "dependencies-demo",
  "dark-theme-demo",
  "events-demo",
  "milestones-tooltips-demo",
  "advanced-demo",
  "localization-demo",
];

const failures = [];
const launchOpts = channel ? { channel, headless: true } : { headless: true };
const browser = await chromium.launch(launchOpts);
const page = await browser.newPage({ viewport: { width: 1400, height: 900 } });

// The sample carries the public demo's licence key, locked to apexcharts.github.io, so on
// localhost the core logs this once per chart. It is expected here and says nothing about the
// wrapper; any other licence message (an expired or malformed key) still fails the run.
const IGNORED_ERRORS = [/^\[Apex\] License is not valid for this domain \(localhost\)\./];

let pageErrors = [];
page.on("console", (m) => {
  if (m.type() !== "error") return;
  const text = m.text();
  if (IGNORED_ERRORS.some((re) => re.test(text))) return;
  pageErrors.push(text.slice(0, 200));
});
page.on("pageerror", (e) => pageErrors.push("PAGEERROR: " + e.message.slice(0, 200)));

// Boot the home page: warms the WASM runtime and asserts the landing page loads clean.
pageErrors = [];
await page.goto(BASE + "/", { waitUntil: "domcontentloaded" });
await page.waitForSelector("nav, .sidebar, .nav-scrollable", { timeout: 90000 }).catch(() => {});
await page.waitForTimeout(3000);
if (pageErrors.length) failures.push(`[(home)] console/page errors: ${JSON.stringify(pageErrors)}`);
console.log(`[(home)] booted errors=${pageErrors.length}`);

async function checkRoute(route) {
  pageErrors = [];
  await page.goto(BASE + "/" + route, { waitUntil: "domcontentloaded" });
  // The gantt renders as HTML: a .gantt-container with task rows and .bar-timeline task bars.
  try { await page.waitForSelector(".gantt-container .bar-timeline", { timeout: 25000 }); } catch { /* asserted below */ }
  await page.waitForTimeout(1500);

  const st = await page.evaluate(() => {
    const eu = document.querySelector("#blazor-error-ui");
    return {
      errUiShown: eu ? getComputedStyle(eu).display !== "none" : false,
      hasContainer: !!document.querySelector(".gantt-container"),
      rows: document.querySelectorAll(".gantt-container .timeline-data-row").length,
      bars: document.querySelectorAll(".gantt-container .bar-timeline").length,
    };
  });

  if (pageErrors.length) failures.push(`[${route}] console/page errors: ${JSON.stringify(pageErrors)}`);
  if (st.errUiShown) failures.push(`[${route}] Blazor error UI is visible`);
  if (!st.hasContainer || st.bars === 0) failures.push(`[${route}] no gantt rendered (container=${st.hasContainer}, rows=${st.rows}, bars=${st.bars})`);
  console.log(`[${route}] container=${st.hasContainer} rows=${st.rows} bars=${st.bars} errUi=${st.errUiShown} errors=${pageErrors.length}`);
}

for (const r of ROUTES) await checkRoute(r);

// A JsFunction crosses the interop as a marker object and is only a function once the bridge
// revives it, so a broken revive still renders the chart and fails on hover instead. Hover a
// bar on the milestones page and expect the custom template's markup, not the built-in one.
async function checkCustomTooltip() {
  const route = "milestones-tooltips-demo";
  pageErrors = [];
  await page.goto(BASE + "/" + route, { waitUntil: "domcontentloaded" });
  await page.waitForSelector(".gantt-container .bar-timeline", { timeout: 25000 });
  await page.locator(".gantt-container .bar-timeline").first().hover();
  const found = await page
    .waitForSelector(".custom-gantt-tooltip", { state: "attached", timeout: 5000 })
    .then(() => true, () => false);
  if (!found) failures.push(`[${route}] hovering a bar did not show the custom TooltipTemplate`);
  if (pageErrors.length) failures.push(`[${route}] console/page errors on hover: ${JSON.stringify(pageErrors)}`);
  console.log(`[${route}] customTooltip=${found} errors=${pageErrors.length}`);
}

await checkCustomTooltip();

// Waits until fn() returns a truthy value (the state unique to the expected outcome), so a slow
// runner never turns into a false failure and a stale state never passes.
async function poll(fn, timeout = 10000) {
  const end = Date.now() + timeout;
  let last;
  while (Date.now() < end) {
    last = await fn();
    if (last) return last;
    await page.waitForTimeout(100);
  }
  return last;
}

// The apexgantt 3.20 methods cross the interop both ways: arguments out, copies of tasks and
// links back into C# models. The events page drives each through a button and prints the
// result, so a missing bridge export, a mis-shaped options object or a payload that does not
// deserialize shows up here.
async function checkApiRoundTrip() {
  const route = "events-demo";
  pageErrors = [];
  await page.goto(BASE + "/" + route, { waitUntil: "domcontentloaded" });
  await page.waitForSelector(".gantt-container .bar-timeline", { timeout: 25000 });
  const status = () => page.locator("#api-status").innerText();
  const entries = () => page.locator(".event-entry").count();
  // Task rows only: the chart pads the space under them with .timeline-empty-row fillers, a
  // frame after each render, so counting those too reads the same total before and after a
  // collapse once the fillers land.
  const rows = () => page.locator(".gantt-container .timeline-data-row:not(.timeline-empty-row)").count();
  const expectStatus = async (label, re) => {
    const ok = await poll(async () => re.test(await status()));
    if (!ok) failures.push(`[${route}] ${label}: status was "${await status()}", expected ${re}`);
  };

  await page.click("#btn-read-data");
  await expectStatus("GetTasksAsync/GetDependenciesAsync/GetTaskAsync", /^5 tasks, 3 links; Testing is at level 2, 30%$/);

  // UpdateTaskAsync raises OnTaskUpdate and OnTaskUpdateSuccess with source "api"
  await page.click("#btn-update-api");
  await expectStatus("UpdateTaskAsync", /^Testing set to 10%$/);
  const apiLogged = await poll(async () => (await page.locator(".event-entry", { hasText: "source: api" }).count()) === 2);
  if (!apiLogged) failures.push(`[${route}] UpdateTaskAsync did not log two events with source: api`);

  // silent: the update lands, no event is raised
  const before = await entries();
  await page.click("#btn-update-silent");
  await expectStatus("UpdateTaskAsync(silent)", /^Testing set to 20% silently$/);
  await page.waitForTimeout(500);
  if ((await entries()) !== before) failures.push(`[${route}] a silent UpdateTaskAsync raised events`);

  // an UpdateAsync with an unchanged task list keeps the chart's edits
  await page.click("#btn-dark");
  await expectStatus("UpdateAsync keeps edits", /^theme Dark; Testing still at 20%$/);

  // collapse hides the four children; ScrollToTaskAsync opens the parent again
  const allRows = await rows();
  await page.click("#btn-collapse");
  await expectStatus("CollapseTaskAsync", /^collapsed: True$/);
  const collapsed = await poll(async () => (await rows()) === allRows - 4);
  if (!collapsed) failures.push(`[${route}] CollapseTaskAsync left ${await rows()} rows (expected ${allRows - 4})`);
  await page.click("#btn-scroll");
  await expectStatus("ScrollToTaskAsync", /^scrolled: True$/);
  const expanded = await poll(async () => (await rows()) === allRows);
  if (!expanded) failures.push(`[${route}] ScrollToTaskAsync did not expand the parent (${await rows()} rows)`);

  if (pageErrors.length) failures.push(`[${route}] console/page errors in the API round trip: ${JSON.stringify(pageErrors)}`);
  console.log(`[${route}] apiRoundTrip status="${await status()}" rows=${await rows()} errors=${pageErrors.length}`);
}

await checkApiRoundTrip();

// LocaleOptions.Pack crosses the interop as a name and only becomes the pack object when the
// bridge imports the bundled locales/ja.es.min.js, so a broken path or name still renders an
// English chart. The page opens at year zoom with Pack = "ja" and a header-only calendar
// (WeekStart only): the year row reads 2026年, never 年度, and no weekend is shaded. The 年度
// box adds FiscalYearStartMonth = 4 (still header-only, still no shading); the second box adds
// a working-calendar property, which must shade the weekends, so the zero above is not vacuous.
async function checkLocalization() {
  const route = "localization-demo";
  pageErrors = [];
  await page.goto(BASE + "/" + route, { waitUntil: "domcontentloaded" });
  await page.waitForSelector(".gantt-container .bar-timeline", { timeout: 25000 });
  const headers = () =>
    page.locator(".gantt-container .timeline-header-cell").evaluateAll((cells) => cells.map((c) => c.textContent.trim()));
  const weekends = () => page.locator(".gantt-container .apexgantt-day--weekend").count();

  const calendarYears = await poll(async () => {
    const h = await headers();
    return h.some((t) => /^\d{4}年$/.test(t)) && !h.some((t) => t.includes("年度")) ? h : null;
  });
  if (!calendarYears) failures.push(`[${route}] Pack = "ja" did not render a 2026年 year row: ${JSON.stringify(await headers())}`);
  const lang = await page.locator(".gantt-container").first().evaluate((el) => el.closest("[lang]")?.getAttribute("lang") ?? null);
  if (lang !== "ja") failures.push(`[${route}] the chart root has lang=${lang}, expected ja`);
  if ((await weekends()) !== 0) failures.push(`[${route}] a WeekStart-only calendar shaded ${await weekends()} weekend days`);

  await page.check("#chk-fiscal");
  const fiscalYears = await poll(async () => {
    const h = await headers();
    return h.some((t) => /^\d{4}年度$/.test(t)) ? h : null;
  });
  if (!fiscalYears) failures.push(`[${route}] FiscalYearStartMonth = 4 did not switch the year row to 年度: ${JSON.stringify(await headers())}`);
  if ((await weekends()) !== 0) failures.push(`[${route}] a WeekStart + FiscalYearStartMonth calendar shaded ${await weekends()} weekend days`);

  await page.check("#chk-weekends");
  const shaded = await poll(async () => (await weekends()) > 0);
  if (!shaded) failures.push(`[${route}] ShowNonWorkingStripes did not turn the working calendar on (no weekend stripes)`);

  if (pageErrors.length) failures.push(`[${route}] console/page errors: ${JSON.stringify(pageErrors)}`);
  console.log(
    `[${route}] lang=${lang} calendarYears=${JSON.stringify((calendarYears || []).filter((t) => t.includes("年")))} ` +
      `fiscalYears=${JSON.stringify((fiscalYears || []).filter((t) => t.includes("年度")))} ` +
      `weekendStripes=${await weekends()} errors=${pageErrors.length}`
  );
}

await checkLocalization();

await browser.close();

if (failures.length) {
  console.error("\nE2E SMOKE FAILED:\n" + failures.map((f) => "  - " + f).join("\n"));
  process.exit(1);
}
console.log("\nE2E smoke passed: all gantt demo pages rendered with no errors, the API round trip held and the ja pack localized the header.");
