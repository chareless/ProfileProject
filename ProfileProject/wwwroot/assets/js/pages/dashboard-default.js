'use strict';
document.addEventListener('DOMContentLoaded', function () {
  setTimeout(function () {
    floatchart();
  }, 500);
});

function floatchart() {
  (function () {
    //var options1 = {
    //  chart: { type: 'bar', height: 50, sparkline: { enabled: true } },
    //  colors: ['#4680FF'],
    //  plotOptions: { bar: { columnWidth: '80%' } },
    //  series: [
    //    {
    //      data: [10, 30, 40, 20, 60, 50, 20, 15, 20, 25, 30, 25]
    //    }
    //  ],
    //  xaxis: { crosshairs: { width: 1 } },
    //  tooltip: {
    //    fixed: { enabled: false },
    //    x: { show: false },
    //    y: {
    //      title: {
    //        formatter: function (seriesName) {
    //          return '';
    //        }
    //      }
    //    },
    //    marker: { show: false }
    //  }
    //};
    //var chart = new ApexCharts(document.querySelector('#all-earnings-graph'), options1);
    //chart.render();
    //var options2 = {
    //  chart: { type: 'bar', height: 50, sparkline: { enabled: true } },
    //  colors: ['#E58A00'],
    //  plotOptions: { bar: { columnWidth: '80%' } },
    //  series: [
    //    {
    //      data: [10, 30, 40, 20, 60, 50, 20, 15, 20, 25, 30, 25]
    //    }
    //  ],
    //  xaxis: { crosshairs: { width: 1 } },
    //  tooltip: {
    //    fixed: { enabled: false },
    //    x: { show: false },
    //    y: {
    //      title: {
    //        formatter: function (seriesName) {
    //          return '';
    //        }
    //      }
    //    },
    //    marker: { show: false }
    //  }
    //};
    //var chart = new ApexCharts(document.querySelector('#page-views-graph'), options2);
    //chart.render();
    //var options3 = {
    //  chart: { type: 'bar', height: 50, sparkline: { enabled: true } },
    //  colors: ['#2CA87F'],
    //  plotOptions: { bar: { columnWidth: '80%' } },
    //  series: [
    //    {
    //      data: [10, 30, 40, 20, 60, 50, 20, 15, 20, 25, 30, 25]
    //    }
    //  ],
    //  xaxis: { crosshairs: { width: 1 } },
    //  tooltip: {
    //    fixed: { enabled: false },
    //    x: { show: false },
    //    y: {
    //      title: {
    //        formatter: function (seriesName) {
    //          return '';
    //        }
    //      }
    //    },
    //    marker: { show: false }
    //  }
    //};
    //var chart = new ApexCharts(document.querySelector('#total-task-graph'), options3);
    //chart.render();
    //var options4 = {
    //  chart: { type: 'bar', height: 50, sparkline: { enabled: true } },
    //  colors: ['#DC2626'],
    //  plotOptions: { bar: { columnWidth: '80%' } },
    //  series: [
    //    {
    //      data: [10, 30, 40, 20, 60, 50, 20, 15, 20, 25, 30, 25]
    //    }
    //  ],
    //  xaxis: { crosshairs: { width: 1 } },
    //  tooltip: {
    //    fixed: { enabled: false },
    //    x: { show: false },
    //    y: {
    //      title: {
    //        formatter: function (seriesName) {
    //          return '';
    //        }
    //      }
    //    },
    //    marker: { show: false }
    //  }
    //};
    //var chart = new ApexCharts(document.querySelector('#download-graph'), options4);
    //chart.render();
    var options5 = {
      chart: {
        type: 'area',
        height: 300,
        toolbar: {
          show: false
        }
      },
      colors: ['#0d6efd'],
      fill: {
        type: 'gradient',
        gradient: {
          shadeIntensity: 1,
          type: 'vertical',
          inverseColors: false,
          opacityFrom: 0.5,
          opacityTo: 0
        }
      },
      dataLabels: {
        enabled: false
      },
      stroke: {
        width: 1
      },
      plotOptions: {
        bar: {
          columnWidth: '45%',
          borderRadius: 4
        }
      },
      grid: {
        strokeDashArray: 4
      },
      series: [
        {
          data: [30, 60, 40, 70, 50, 90, 50, 55, 45, 60, 50, 65]
        }
      ],
      xaxis: {
        categories: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'],
        axisBorder: {
          show: false
        },
        axisTicks: {
          show: false
        }
      }
    };
    var chart = new ApexCharts(document.querySelector('#customer-rate-graph'), options5);
    chart.render();
    var options6 = {
      chart: {
        type: 'area',
        height: 60,
        stacked: true,
        sparkline: { enabled: true }
      },
      colors: ['#4680FF'],
      fill: {
        type: 'gradient',
        gradient: {
          shadeIntensity: 1,
          type: 'vertical',
          inverseColors: false,
          opacityFrom: 0.5,
          opacityTo: 0
        }
      },
      stroke: { curve: 'smooth', width: 2 },
      series: [{ data: [5, 25, 3, 10, 4, 50, 0] }]
    };
    var chart = new ApexCharts(document.querySelector('#total-tasks-graph'), options6);
    chart.render();
    var options7 = {
      chart: {
        type: 'area',
        height: 60,
        stacked: true,
        sparkline: { enabled: true }
      },
      colors: ['#DC2626'],
      fill: {
        type: 'gradient',
        gradient: {
          shadeIntensity: 1,
          type: 'vertical',
          inverseColors: false,
          opacityFrom: 0.5,
          opacityTo: 0
        }
      },
      stroke: { curve: 'smooth', width: 2 },
      series: [{ data: [0, 50, 4, 10, 3, 25, 5] }]
    };
    var chart = new ApexCharts(document.querySelector('#pending-tasks-graph'), options7);
    chart.render();
    var options8 = {
      chart: {
        height: 320,
        type: 'donut'
      },
      series: [27, 23, 20, 17],
      colors: ['#4680FF', '#E58A00', '#2CA87F', '#4680FF'],
      labels: ['Total income', 'Total rent', 'Download', 'Views'],
      fill: {
        opacity: [1, 1, 1, 0.3]
      },
      legend: {
        show: false
      },
      plotOptions: {
        pie: {
          donut: {
            size: '65%',
            labels: {
              show: true,
              name: {
                show: true
              },
              value: {
                show: true
              }
            }
          }
        }
      },
      dataLabels: {
        enabled: false
      },
      responsive: [
        {
          breakpoint: 480,
          options: {
            plotOptions: {
              pie: {
                donut: {
                  size: '65%',
                  labels: {
                    show: true
                  }
                }
              }
            }
          }
        }
      ]
    };
    var chart = new ApexCharts(document.querySelector('#total-income-graph'), options8);
    chart.render();
  })();
}
