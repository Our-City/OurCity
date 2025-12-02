<script setup lang="ts">
import { ref, watch, onMounted } from "vue";
import { CChart } from "@coreui/vue-chartjs";
import { Period } from "@/types/enums";
import { getAnalyticsTimeSeries } from "@/api/analyticsService";

const props = defineProps({
  period: Period,
});

const chartData = ref({
  labels: [],
  datasets: [
    {
      label: "Posts Created",
      backgroundColor: "rgba(2, 101, 194, 0.1)",
      borderColor: "rgba(2, 101, 194, 1)",
      pointBackgroundColor: "rgba(2, 101, 194, 1)",
      pointBorderColor: "#fff",
      pointHoverBackgroundColor: "#fff",
      pointHoverBorderColor: "rgba(2, 101, 194, 1)",
      data: [],
      fill: true,
      tension: 0.4,
    },
  ],
});

const chartOptions = ref({
  responsive: true,
  maintainAspectRatio: false,
  plugins: {
    legend: {
      display: false,
    },
    tooltip: {
      backgroundColor: "rgba(0, 0, 0, 0.8)",
      padding: 12,
      titleColor: "#fff",
      bodyColor: "#fff",
      displayColors: false,
    },
  },
  scales: {
    x: {
      grid: {
        display: false,
      },
      ticks: {
        maxRotation: 45,
        minRotation: 0,
      },
    },
    y: {
      beginAtZero: true,
      grid: {
        color: "rgba(0, 0, 0, 0.05)",
      },
      ticks: {
        precision: 0,
      },
    },
  },
});

const isLoading = ref(false);
const error = ref<string | null>(null);
const hasData = ref(true);

const fetchChartData = async () => {
  try {
    isLoading.value = true;
    error.value = null;

    const response = await getAnalyticsTimeSeries(props.period);

    // Check if there's any data
    const totalPosts = response.series.reduce((sum, entry) => sum + entry.postCount, 0);
    hasData.value = totalPosts > 0;

    let labels: string[] = [];
    let data: number[] = [];

    if (props.period === Period.Day) {
      labels = response.series.map((entry) => entry.bucketStart.toISOString().slice(11, 16));
      data = response.series.map((entry) => entry.postCount);
    } else if (props.period === Period.Month) {
      labels = response.series.map((entry) => entry.bucketStart.toISOString().slice(0, 10));
      data = response.series.map((entry) => entry.postCount);
    } else if (props.period === Period.Year) {
      labels = response.series.map((entry) => entry.bucketStart.toISOString().slice(0, 7));
      data = response.series.map((entry) => entry.postCount);
    }

    // Create new object to trigger reactivity
    chartData.value = {
      labels: labels,
      datasets: [
        {
          label: "Posts Created",
          backgroundColor: "rgba(2, 101, 194, 0.1)",
          borderColor: "rgba(2, 101, 194, 1)",
          pointBackgroundColor: "rgba(2, 101, 194, 1)",
          pointBorderColor: "#fff",
          pointHoverBackgroundColor: "#fff",
          pointHoverBorderColor: "rgba(2, 101, 194, 1)",
          data: data,
          fill: true,
          tension: 0.4,
        },
      ],
    };
  } catch (err) {
    console.error("Failed to fetch chart data:", err);
    error.value = "Failed to fetch chart data";
  } finally {
    isLoading.value = false;
  }
};

// Watch for period changes
watch(
  () => props.period,
  () => {
    fetchChartData();
  },
);

onMounted(() => {
  fetchChartData();
});
</script>

<template>
  <h4>Posts Over Time</h4>
  <div v-if="isLoading" class="empty-state">
    <div class="spinner"></div>
    <p>Loading chart data...</p>
  </div>
  <div v-else-if="error" class="empty-state">
    <i class="pi pi-exclamation-circle" style="font-size: 2rem; margin-bottom: 0.5rem; color: var(--negative-color);"></i>
    <p>{{ error }}</p>
  </div>
  <div v-else-if="!hasData" class="empty-state">
    <i class="pi pi-chart-line" style="font-size: 2rem; margin-bottom: 0.5rem; color: var(--tertiary-text-color);"></i>
    <p>No posts created during this period</p>
  </div>
  <div v-else class="chart-wrapper">
    <CChart type="line" :data="chartData" :options="chartOptions" />
  </div>
</template>

<style scoped>
.chart-wrapper {
  padding: 1rem;
  background: white;
  overflow: hidden;
  height: 400px;
}

.empty-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  text-align: center;
  color: var(--tertiary-text-color);
  padding: 2rem;
}

.empty-state p {
  font-size: 1rem;
  margin: 0;
}

.spinner {
  width: 40px;
  height: 40px;
  border: 3px solid rgba(0, 0, 0, 0.1);
  border-left-color: var(--neutral-color);
  border-radius: 50%;
  animation: spin 1s linear infinite;
  margin-bottom: 0.5rem;
}

@keyframes spin {
  to {
    transform: rotate(360deg);
  }
}

@media (max-width: 768px) {
  .chart-wrapper {
    height: 290px;
  }
}
</style>
