/*! Blazor-ApexGantt interop bridge (ES module) */
//
// Self-contained: imports the vendored apexgantt ESM dist directly, so the host app
// no longer needs a <script> tag or a CDN reference for the core library.
import { ApexGantt } from "./apexgantt.es.min.js?ver=3.15.0";

// chart instances and their event-listener cleanups, keyed by element id
const instances = {};
const cleanups = {};

// core CustomEvent name -> Blazor [JSInvokable] handler method
const EVENT_HANDLERS = {
  taskUpdate: "HandleTaskUpdate",
  taskUpdateSuccess: "HandleTaskUpdateSuccess",
  taskUpdateError: "HandleTaskUpdateError",
  taskValidationError: "HandleTaskValidationError",
  taskDragged: "HandleTaskDragged",
  taskResized: "HandleTaskResized",
  taskAdded: "HandleTaskAdded",
  taskDeleted: "HandleTaskDeleted",
  taskMoved: "HandleTaskMoved",
  taskProgressChanged: "HandleTaskProgressChanged",
  dependencyAdded: "HandleDependencyAdded",
  dependencyRemoved: "HandleDependencyRemoved",
  selectionChange: "HandleSelectionChange",
  dependencyArrowUpdate: "HandleDependencyArrowUpdate",
  historyChange: "HandleHistoryChange",
  sortChange: "HandleSortChange",
  filterChange: "HandleFilterChange",
  groupChange: "HandleGroupChange",
  columnResize: "HandleColumnResize",
  columnReorder: "HandleColumnReorder",
};

// Map a CustomEvent detail into a plain, JSON-serializable payload for .NET.
// Most detail objects are already plain data that maps 1:1 to the C# event args;
// the Error object on taskUpdateError is the one that needs flattening.
function toPayload(name, detail) {
  if (!detail) return {};
  if (name === "taskUpdateError") {
    return {
      taskId: detail.taskId,
      errorMessage: detail.error?.message ?? "Unknown error",
      timestamp: detail.timestamp,
    };
  }
  return detail;
}

function chartOf(elementId) {
  const chart = instances[elementId];
  if (!chart) throw new Error(`ApexGantt instance not found: ${elementId}`);
  return chart;
}

export function setLicense(licenseKey) {
  try {
    if (ApexGantt && typeof ApexGantt.setLicense === "function") {
      ApexGantt.setLicense(licenseKey);
      return true;
    }
    console.error("ApexGantt.setLicense is not available");
    return false;
  } catch (error) {
    console.error("failed to set apexgantt license:", error);
    return false;
  }
}

export function init(elementId, optionsJson, dotNetRef) {
  try {
    const element = document.getElementById(elementId);
    if (!element) {
      console.error("element not found:", elementId);
      return false;
    }

    const options = JSON.parse(optionsJson);

    // resolve percentage/auto width to actual pixels against the container
    if (
      typeof options.width === "string" &&
      (options.width.includes("%") || options.width === "auto")
    ) {
      const container = element.parentElement;
      if (container) {
        const style = window.getComputedStyle(container);
        const paddingLeft = parseFloat(style.paddingLeft) || 0;
        const paddingRight = parseFloat(style.paddingRight) || 0;
        options.width = container.clientWidth - paddingLeft - paddingRight;
      }
    }

    const chart = new ApexGantt(element, options);
    chart.render();
    instances[elementId] = chart;

    if (dotNetRef) {
      const added = [];
      for (const [eventName, handler] of Object.entries(EVENT_HANDLERS)) {
        const listener = (e) =>
          dotNetRef.invokeMethodAsync(handler, toPayload(eventName, e.detail));
        element.addEventListener(eventName, listener);
        added.push([eventName, listener]);
      }
      cleanups[elementId] = () =>
        added.forEach(([eventName, listener]) =>
          element.removeEventListener(eventName, listener)
        );
    }

    return true;
  } catch (error) {
    console.error("failed to initialize apexgantt:", error);
    return false;
  }
}

export function update(elementId, optionsJson) {
  chartOf(elementId).update(JSON.parse(optionsJson));
}

export function destroy(elementId) {
  try {
    cleanups[elementId]?.();
    delete cleanups[elementId];
    const chart = instances[elementId];
    if (chart) {
      chart.destroy();
      delete instances[elementId];
    }
    return true;
  } catch (error) {
    console.error("failed to destroy chart:", error);
    return false;
  }
}

// --- Task CRUD ---------------------------------------------------------------
export function addTask(elementId, taskJson) {
  chartOf(elementId).addTask(JSON.parse(taskJson));
}
export function updateTask(elementId, taskId, updatesJson) {
  chartOf(elementId).updateTask(taskId, JSON.parse(updatesJson));
}
export function deleteTask(elementId, taskId) {
  chartOf(elementId).deleteTask(taskId);
}
export function moveTask(elementId, taskId, optionsJson) {
  chartOf(elementId).moveTask(taskId, optionsJson ? JSON.parse(optionsJson) : undefined);
}

// --- Dependencies ------------------------------------------------------------
export function addDependency(elementId, fromId, toId, optionsJson) {
  return chartOf(elementId).addDependency(
    fromId,
    toId,
    optionsJson ? JSON.parse(optionsJson) : undefined
  );
}
export function removeDependency(elementId, fromId, toId) {
  return chartOf(elementId).removeDependency(fromId, toId);
}

// --- History -----------------------------------------------------------------
export function undo(elementId) {
  return chartOf(elementId).undo();
}
export function redo(elementId) {
  return chartOf(elementId).redo();
}
export function canUndo(elementId) {
  return chartOf(elementId).canUndo();
}
export function canRedo(elementId) {
  return chartOf(elementId).canRedo();
}
export function clearHistory(elementId) {
  chartOf(elementId).clearHistory();
}

// --- Sort / group / filter ---------------------------------------------------
export function sort(elementId, criteriaJson) {
  chartOf(elementId).sort(JSON.parse(criteriaJson));
}
export function clearSort(elementId) {
  chartOf(elementId).clearSort();
}
export function groupBy(elementId, criterion) {
  chartOf(elementId).groupBy(criterion);
}
export function clearGrouping(elementId) {
  chartOf(elementId).clearGrouping();
}
export function setFilterRules(elementId, rulesJson) {
  chartOf(elementId).setFilterRules(rulesJson ? JSON.parse(rulesJson) : null);
}
export function clearFilter(elementId) {
  chartOf(elementId).clearFilter();
}

// --- Columns -----------------------------------------------------------------
export function setColumnWidth(elementId, key, width) {
  chartOf(elementId).setColumnWidth(key, width);
}
export function resetColumnWidths(elementId, key) {
  chartOf(elementId).resetColumnWidths(key || undefined);
}
export function setColumnOrder(elementId, orderJson) {
  chartOf(elementId).setColumnOrder(JSON.parse(orderJson));
}
export function getColumnOrder(elementId) {
  return chartOf(elementId).getColumnOrder();
}

// --- Selection / navigation / zoom / export ----------------------------------
export function scrollToTask(elementId, taskId) {
  return chartOf(elementId).scrollToTask(taskId);
}
export function getSelectedTasks(elementId) {
  return chartOf(elementId).getSelectedTasks();
}
export function setSelectedTasks(elementId, idsJson) {
  chartOf(elementId).setSelectedTasks(JSON.parse(idsJson));
}
export function clearSelection(elementId) {
  chartOf(elementId).clearSelection();
}
export function zoomIn(elementId) {
  chartOf(elementId).zoomIn();
}
export function zoomOut(elementId) {
  chartOf(elementId).zoomOut();
}
export function exportChart(elementId, format) {
  return chartOf(elementId).exportChart(format || undefined);
}
