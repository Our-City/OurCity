<script setup lang="ts">
import { ref, watch, onMounted } from "vue";
import { CChart } from "@coreui/vue-chartjs";
import { getAnalyticsTags } from "@/api/analyticsService";
import { Period } from "@/types/enums";

const props = defineProps<{
  period: Period;
}>();

const chartData = ref<{
  labels: string[];
  datasets: Array<{
    label: string;
    data: number[];
    backgroundColor?: string[];
    borderColor?: string[];
    borderWidth: number;
  }>;
}>({
  labels: [],
  datasets: [
    {
      label: "Posts by Tag",
      data: [],
      backgroundColor: [],
      borderColor: [],
      borderWidth: 2,
    },
  ],
});

const chartOptions = ref({
  responsive: true,
  maintainAspectRatio: false,
  plugins: {
    legend: {
      position: "right" as const,
      labels: {
        padding: 15,
        font: {
          size: 12,
        },
      },
    },
    tooltip: {
      enabled: true,
      external: undefined,
      backgroundColor: "rgba(0, 0, 0, 0.8)",
      padding: 12,
      titleColor: "#fff",
      bodyColor: "#fff",
      callbacks: {
        label: function (context: { label: string; parsed: number; dataset: { data: number[] } }) {
          const label = context.label || "";
          const value = context.parsed || 0;
          const total = context.dataset.data.reduce((a: number, b: number) => a + b, 0);
          const percentage = ((value / total) * 100).toFixed(1);
          return `${label}: ${value} (${percentage}%)`;
        },
      },
    },
  },
});

const isLoading = ref(true);
const error = ref<string | null>(null);

// Load tags and generate mock post counts
const loadTagData = async () => {
  try {
    isLoading.value = true;
    error.value = null;

    const response = await getAnalyticsTags(props.period);
    const labels = response.tagBuckets.map((tag) => tag.tagName);
    const data = response.tagBuckets.map((tag) => tag.postCount);

    // Generate colors for each tag
    const colors = [
      "#0265C2",
      "#001627",
      "#3385D0",
      "#013F7A",
      "#66A5DD",
      "#99C4E9",
      "#0153A0",
      "#002A52",
    ];

    const borderColors = [
      "#0265C2",
      "#001627",
      "#3385D0",
      "#013F7A",
      "#66A5DD",
      "#99C4E9",
      "#0153A0",
      "#002A52",
    ];

    // Repeat colors if there are more tags than colors
    const backgroundColor = labels.map((_, i) => colors[i % colors.length]);
    const borderColor = labels.map((_, i) => borderColors[i % borderColors.length]);

    chartData.value = {
      labels: labels,
      datasets: [
        {
          label: "Posts by Tag",
          data: data,
          backgroundColor: backgroundColor,
          borderColor: borderColor,
          borderWidth: 2,
        },
      ],
    };
  } catch (err) {
    console.error("Failed to load tag data:", err);
    error.value = "Failed to load tag data";
  } finally {
    isLoading.value = false;
  }
};

// Watch for period changes
watch(
  () => props.period,
  () => {
    loadTagData();
  },
);

onMounted(() => {
  loadTagData();
});
</script>

<template>
  <h4>Tag Breakdown</h4>
  <div class="chart-container">
    <div v-if="isLoading" class="loading-state">
      <div class="spinner"></div>
      <p>Loading tag data...</p>
    </div>
    <div v-else-if="error" class="error-state">
      <p>{{ error }}</p>
    </div>
    <CChart type="doughnut" :data="chartData" :options="chartOptions" />
  </div>
</template>

<style scoped>
.chart-wrapper {
  position: relative;
  height: 600px;
  max-height: 600px;
  padding: 1rem;
  overflow: hidden;
}

.chart-container {
  position: relative;
  width: 100%;
  overflow: hidden;
}

.loading-state,
.error-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  min-height: 200px;
  color: var(--tertiary-text-color);
}

.spinner {
  border: 3px solid rgba(0, 0, 0, 0.1);
  border-left-color: var(--neutral-color);
  border-radius: 50%;
  width: 40px;
  height: 40px;
  animation: spin 1s linear infinite;
  margin-bottom: 1rem;
}

@keyframes spin {
  to {
    transform: rotate(360deg);
  }
}

.error-state p {
  color: var(--negative-color);
  font-weight: 500;
}

@media (max-width: 768px) {
  .chart-wrapper {
    max-height: 340px;
  }

  .chart-container {
    height: 300px;
    max-height: 300px;
  }

  .chart-container :deep(canvas) {
    max-height: 300px !important;
  }

  .chart-container :deep(.legend) {
    font-size: 10px;
  }
}
</style>
