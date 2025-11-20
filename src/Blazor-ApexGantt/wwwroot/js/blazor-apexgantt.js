// blazor interop bridge for apexgantt
window.blazorApexGantt = {
  // store chart instances and event listeners
  instances: {},
  eventListeners: {},

  // initialize a new gantt chart
  init: function (elementId, options, dotNetRef) {
    try {
      const element = document.getElementById(elementId);
      if (!element) {
        console.error("element not found:", elementId);
        return false;
      }

      // if width is percentage or auto, calculate actual pixels
      if (
        typeof options.width === "string" &&
        (options.width.includes("%") || options.width === "auto")
      ) {
        const container = element.parentElement;
        if (container) {
          // get container width minus padding
          const style = window.getComputedStyle(container);
          const paddingLeft = parseFloat(style.paddingLeft) || 0;
          const paddingRight = parseFloat(style.paddingRight) || 0;
          options.width = container.clientWidth - paddingLeft - paddingRight;
        }
      }

      // create new apexgantt instance
      const chart = new ApexGantt(element, options);

      // render the chart
      chart.render();

      // store instance for future reference
      this.instances[elementId] = chart;

      // setup event listeners if dotNetRef is provided
      if (dotNetRef) {
        this.setupEventListeners(elementId, element, dotNetRef);
      }

      return true;
    } catch (error) {
      console.error("failed to initialize apexgantt:", error);
      return false;
    }
  },

  // setup event listeners for apexgantt events
  setupEventListeners: function (elementId, element, dotNetRef) {
    const listeners = {};

    // task update event
    const taskUpdateListener = (event) => {
      const detail = event.detail;
      dotNetRef.invokeMethodAsync("HandleTaskUpdate", {
        taskId: detail.taskId,
        updatedTask: this.mapTaskToBlazor(detail.updatedTask),
        timestamp: detail.timestamp,
      });
    };

    // task update success event
    const taskUpdateSuccessListener = (event) => {
      const detail = event.detail;
      dotNetRef.invokeMethodAsync("HandleTaskUpdateSuccess", {
        taskId: detail.taskId,
        updatedTask: this.mapTaskToBlazor(detail.updatedTask),
        timestamp: detail.timestamp,
      });
    };

    // task update error event
    const taskUpdateErrorListener = (event) => {
      const detail = event.detail;
      dotNetRef.invokeMethodAsync("HandleTaskUpdateError", {
        taskId: detail.taskId,
        errorMessage: detail.error?.message || "Unknown error",
        timestamp: detail.timestamp,
      });
    };

    // task validation error event
    const taskValidationErrorListener = (event) => {
      const detail = event.detail;
      dotNetRef.invokeMethodAsync("HandleTaskValidationError", {
        taskId: detail.taskId,
        errors: detail.errors || [],
        timestamp: detail.timestamp,
      });
    };

    // add event listeners
    element.addEventListener("taskUpdate", taskUpdateListener);
    element.addEventListener("taskUpdateSuccess", taskUpdateSuccessListener);
    element.addEventListener("taskUpdateError", taskUpdateErrorListener);
    element.addEventListener(
      "taskValidationError",
      taskValidationErrorListener
    );

    // store listeners for cleanup
    listeners.taskUpdate = taskUpdateListener;
    listeners.taskUpdateSuccess = taskUpdateSuccessListener;
    listeners.taskUpdateError = taskUpdateErrorListener;
    listeners.taskValidationError = taskValidationErrorListener;

    this.eventListeners[elementId] = {
      element: element,
      listeners: listeners,
    };
  },

  // map task object from JS to Blazor-friendly format
  mapTaskToBlazor: function (task) {
    if (!task) return null;

    return {
      id: task.id,
      name: task.name,
      startTime: task.startTime,
      endTime: task.endTime,
      progress: task.progress,
      parentId: task.parentId,
      dependency: task.dependency,
      color: task.color,
      className: task.className,
    };
  },

  // update existing chart
  update: function (elementId, options) {
    try {
      const chart = this.instances[elementId];
      if (!chart) {
        console.error("chart instance not found:", elementId);
        return false;
      }

      chart.updateOptions(options);
      return true;
    } catch (error) {
      console.error("failed to update chart:", error);
      return false;
    }
  },

  // destroy chart and cleanup
  destroy: function (elementId) {
    try {
      // cleanup event listeners
      const eventData = this.eventListeners[elementId];
      if (eventData) {
        const { element, listeners } = eventData;
        element.removeEventListener("taskUpdate", listeners.taskUpdate);
        element.removeEventListener(
          "taskUpdateSuccess",
          listeners.taskUpdateSuccess
        );
        element.removeEventListener(
          "taskUpdateError",
          listeners.taskUpdateError
        );
        element.removeEventListener(
          "taskValidationError",
          listeners.taskValidationError
        );
        delete this.eventListeners[elementId];
      }

      // destroy chart
      const chart = this.instances[elementId];
      if (chart) {
        chart.destroy();
        delete this.instances[elementId];
      }
      return true;
    } catch (error) {
      console.error("failed to destroy chart:", error);
      return false;
    }
  },
};
