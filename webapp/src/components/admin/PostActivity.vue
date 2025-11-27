<script setup>
import { ref, onMounted } from "vue";
import { CContainer, CRow, CCol, CCard, CCardBody, CCardHeader, CButtonGroup, CButton } from "@coreui/vue";
import PostsOverTime from "./Charts/PostsOverTime.vue";
import TagBreakdown from "./Charts/TagBreakdown.vue";

// Time period selection
const selectedPeriod = ref('day');

// Stats data
const stats = ref({
  newPosts: 0,
  upvotes: 0,
  downvotes: 0,
  comments: 0
});

// Handle period selection
const selectPeriod = (period) => {
  selectedPeriod.value = period;
  fetchStats();
};

// Fetch stats based on selected period
const fetchStats = async () => {
  // TODO: Replace with actual API call that uses selectedPeriod.value
  // Simulating API call with mock data that changes based on period
  const mockData = {
    day: { newPosts: 24, upvotes: 156, downvotes: 23, comments: 89 },
    month: { newPosts: 342, upvotes: 2145, downvotes: 287, comments: 1234 },
    year: { newPosts: 4128, upvotes: 25896, downvotes: 3452, comments: 14890 }
  };
  
  stats.value = mockData[selectedPeriod.value];
};

// Fetch stats on mount
onMounted(() => {
  fetchStats();
});
</script>

<template>
  <CContainer>
    <CCard class="stats-wrapper">
      <CCardHeader>
        <CRow :xs="{ gutterY: 5 }">
          <CCol>
            <h3>Activity</h3>
          </CCol>
          <CCol class="text-end">
            <CButtonGroup>
              <CButton 
                :class="{ active: selectedPeriod === 'day' }"
                @click="selectPeriod('day')"
              >
                Day
              </CButton>
              <CButton 
                :class="{ active: selectedPeriod === 'month' }"
                @click="selectPeriod('month')"
              >
                Month
              </CButton>
              <CButton 
                :class="{ active: selectedPeriod === 'year' }"
                @click="selectPeriod('year')"
              >
                Year
              </CButton>
            </CButtonGroup>
          </CCol>
        </CRow>
      </CCardHeader>

      <CCardBody>
        <CRow :xs="{ cols: 1, gutterY: 5 }">
          <CCol>
            <CRow :s="{ cols: 1, gutterY: 5 }" :m="{ cols: 2}" :xl="{ cols: 4 }">
              <CCol>
                <CCard class="stat-card new-posts">
                  <CCardBody>
                    <div class="stat-content">
                      <div class="stat-value">{{ stats.newPosts }}</div>
                      <div class="stat-label">New Posts</div>
                    </div>
                  </CCardBody>
                </CCard>
              </CCol>
              <CCol>
                <CCard class="stat-card upvotes">
                  <CCardBody>
                    <div class="stat-content">
                      <div class="stat-value">{{ stats.upvotes }}</div>
                      <div class="stat-label">Upvotes</div>
                    </div>
                  </CCardBody>
                </CCard>
              </CCol>
              <CCol>
                <CCard class="stat-card downvotes">
                  <CCardBody>
                    <div class="stat-content">
                      <div class="stat-value">{{ stats.downvotes }}</div>
                      <div class="stat-label">Downvotes</div>
                    </div>
                  </CCardBody>
                </CCard>
              </CCol>
              <CCol>
                <CCard class="stat-card comments">
                  <CCardBody>
                    <div class="stat-content">
                      <div class="stat-value">{{ stats.comments }}</div>
                      <div class="stat-label">Comments</div>
                    </div>
                  </CCardBody>
                </CCard>
              </CCol>
            </CRow>
          </CCol>
          <CCol>
            <PostsOverTime :period="selectedPeriod"/>
          </CCol>
          <CCol>
            <TagBreakdown/>
          </CCol>
        </CRow>
      </CCardBody>
    </CCard>
  </CContainer>
</template>

<style scoped>
.stats-wrapper {
  background: white;
  border-radius: 0.75rem;
  border: none;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
  width: 100%;
}

.stat-card {
  margin-bottom: 1rem;
  border: none;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

.stat-card :deep(.card-body) {
  padding: 1.5rem;
}

.stat-card.new-posts {
  background: linear-gradient(135deg, var(--neutral-color), var(--neutral-color-hover));
}

.stat-card.upvotes {
  background: linear-gradient(135deg, var(--positive-color), #0d9668);
}

.stat-card.downvotes {
  background: linear-gradient(135deg, var(--negative-color), #dc2626);
}

.stat-card.comments {
  background: linear-gradient(135deg, var(--tertiary-text-color), #3f444a);
}

.stat-content {
  flex: 1;
}

.stat-value {
  font-size: 2rem;
  font-weight: 700;
  color: white;
  line-height: 1;
  margin-bottom: 0.25rem;
}

.stat-label {
  font-size: 0.875rem;
  color: rgba(255, 255, 255, 0.9);
  text-transform: uppercase;
  letter-spacing: 0.5px;
  font-weight: 500;
}

:deep(.btn-group) {
  border-radius: 0.375rem;
  overflow: hidden;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
}

:deep(.btn-group .btn) {
  background: var(--primary-background-color);
  color: var(--primary-text-color);
  border: 1px solid var(--tertiary-text-color);
  padding: 0.5rem 1rem;
  font-size: 0.875rem;
  font-weight: 500;
  transition: all 0.2s ease;
}

:deep(.btn-group .btn:hover) {
  background: var(--secondary-background-color);
  color: var(--primary-text-color);
}

:deep(.btn-group .btn.active) {
  background: var(--tertiary-text-color);
  color: white;
  border-color: var(--tertiary-text-color);
}

@media (max-width: 576px) {
  .stat-card :deep(.card-body) {
    padding: 1rem;
  }

  .stat-value {
    font-size: 1.5rem;
  }
}
</style>