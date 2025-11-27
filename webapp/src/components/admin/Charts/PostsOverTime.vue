<script setup>
import { ref, watch, onMounted } from "vue";
import { CChart } from "@coreui/vue-chartjs";

const props = defineProps({
  period: {
    type: String,
    required: true,
    validator: (value) => ['day', 'month', 'year'].includes(value)
  }
});

const chartData = ref({
  labels: [],
  datasets: [
    {
      label: 'Posts Created',
      backgroundColor: 'rgba(2, 101, 194, 0.1)',
      borderColor: 'rgba(2, 101, 194, 1)',
      pointBackgroundColor: 'rgba(2, 101, 194, 1)',
      pointBorderColor: '#fff',
      pointHoverBackgroundColor: '#fff',
      pointHoverBorderColor: 'rgba(2, 101, 194, 1)',
      data: [],
      fill: true,
      tension: 0.4
    }
  ]
});

const chartOptions = ref({
  responsive: true,
  maintainAspectRatio: false,
  plugins: {
    legend: {
      display: false
    },
    tooltip: {
      backgroundColor: 'rgba(0, 0, 0, 0.8)',
      padding: 12,
      titleColor: '#fff',
      bodyColor: '#fff',
      displayColors: false
    }
  },
  scales: {
    x: {
      grid: {
        display: false
      },
      ticks: {
        maxRotation: 45,
        minRotation: 0
      }
    },
    y: {
      beginAtZero: true,
      grid: {
        color: 'rgba(0, 0, 0, 0.05)'
      },
      ticks: {
        precision: 0
      }
    }
  }
});

// Generate labels and mock data based on period
const generateChartData = () => {
  let labels = [];
  let data = [];

  if (props.period === 'day') {
    // Hourly for last 24 hours
    for (let i = 23; i >= 0; i--) {
      const hour = new Date();
      hour.setHours(hour.getHours() - i);
      labels.push(hour.getHours() + ':00');
      data.push(Math.floor(Math.random() * 10)); // Mock data
    }
  } else if (props.period === 'month') {
    // Daily for last 30 days
    for (let i = 29; i >= 0; i--) {
      const date = new Date();
      date.setDate(date.getDate() - i);
      labels.push((date.getMonth() + 1) + '/' + date.getDate());
      data.push(Math.floor(Math.random() * 20)); // Mock data
    }
  } else if (props.period === 'year') {
    // Monthly for last 12 months
    const monthNames = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
    for (let i = 11; i >= 0; i--) {
      const date = new Date();
      date.setMonth(date.getMonth() - i);
      labels.push(monthNames[date.getMonth()]);
      data.push(Math.floor(Math.random() * 500)); // Mock data
    }
  }

  // Create new object to trigger reactivity
  chartData.value = {
    labels: labels,
    datasets: [{
      label: 'Posts Created',
      backgroundColor: 'rgba(2, 101, 194, 0.1)',
      borderColor: 'rgba(2, 101, 194, 1)',
      pointBackgroundColor: 'rgba(2, 101, 194, 1)',
      pointBorderColor: '#fff',
      pointHoverBackgroundColor: '#fff',
      pointHoverBorderColor: 'rgba(2, 101, 194, 1)',
      data: data,
      fill: true,
      tension: 0.4
    }]
  };
};

// Watch for period changes
watch(() => props.period, () => {
  generateChartData();
});

onMounted(() => {
  generateChartData();
});
</script>

<template>
    <h4>Posts Over Time</h4>
    <div class="chart-wrapper">
        <CChart
            type="line"
            :data="chartData"
            :options="chartOptions"
        />
    </div>
</template>

<style scoped>
.chart-wrapper {
  padding: 1rem;
  background: white;
  overflow: hidden;
  height: 400px;
}

@media (max-width: 768px) {
  .chart-wrapper {
    height: 290px;
  }
}
</style>
