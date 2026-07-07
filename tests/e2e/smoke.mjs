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
  "advanced-demo",
];

const failures = [];
const launchOpts = channel ? { channel, headless: true } : { headless: true };
const browser = await chromium.launch(launchOpts);
const page = await browser.newPage({ viewport: { width: 1400, height: 900 } });

let pageErrors = [];
page.on("console", (m) => { if (m.type() === "error") pageErrors.push(m.text().slice(0, 200)); });
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

await browser.close();

if (failures.length) {
  console.error("\nE2E SMOKE FAILED:\n" + failures.map((f) => "  - " + f).join("\n"));
  process.exit(1);
}
console.log("\nE2E smoke passed: all gantt demo pages rendered with no errors.");
