// blazor interop bridge for apexgantt
window.blazorApexGantt = {
  // store chart instances
  instances: {},

  // initialize a new gantt chart
  init: function (elementId, options) {
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

      return true;
    } catch (error) {
      console.error("failed to initialize apexgantt:", error);
      return false;
    }
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
