<script setup lang="ts">
interface Props {
  title?: string
  subtitle?: string
  variant?: 'default' | 'card' | 'modal' | 'inline'
  width?: 'narrow' | 'medium' | 'wide' | 'full'
  loading?: boolean
  disabled?: boolean
}

interface Emits {
  submit: [event: Event]
  reset: [event: Event]
}

const props = withDefaults(defineProps<Props>(), {
  variant: 'default',
  width: 'medium',
  loading: false,
  disabled: false
})

const emit = defineEmits<Emits>()

const handleSubmit = (event: Event) => {
  if (props.disabled || props.loading) {
    event.preventDefault()
    return
  }
  emit('submit', event)
}

const handleReset = (event: Event) => {
  if (props.disabled || props.loading) {
    event.preventDefault()
    return
  }
  emit('reset', event)
}
</script>

<template>
  <div :class="['form-container', `form-container--${variant}`, `form-container--${width}`]">
    <!-- Form Header -->
    <div v-if="title || subtitle || $slots.header" class="form-header">
      <slot name="header">
        <h2 v-if="title" class="form-title">{{ title }}</h2>
        <p v-if="subtitle" class="form-subtitle">{{ subtitle }}</p>
      </slot>
    </div>

    <!-- Form Content -->
    <form 
      :class="['form', { 'form--loading': loading, 'form--disabled': disabled }]"
      @submit.prevent="handleSubmit"
      @reset.prevent="handleReset"
    >
      <!-- Before Fields Slot -->
      <div v-if="$slots.before" class="form-section form-section--before">
        <slot name="before"></slot>
      </div>

      <!-- Main Form Fields -->
      <div class="form-fields">
        <slot></slot>
      </div>

      <!-- Between Fields and Actions Slot -->
      <div v-if="$slots.between" class="form-section form-section--between">
        <slot name="between"></slot>
      </div>

      <!-- Form Actions -->
      <div v-if="$slots.actions" class="form-actions">
        <slot name="actions" :loading="loading" :disabled="disabled"></slot>
      </div>

      <!-- After Actions Slot -->
      <div v-if="$slots.after" class="form-section form-section--after">
        <slot name="after"></slot>
      </div>
    </form>

    <!-- Form Footer -->
    <div v-if="$slots.footer" class="form-footer">
      <slot name="footer"></slot>
    </div>

    <!-- Loading Overlay -->
    <div v-if="loading" class="form-loading-overlay">
      <div class="form-spinner">
        <i class="pi pi-spin pi-spinner"></i>
      </div>
    </div>
  </div>
</template>

<style scoped>
.form-container {
  position: relative;
  width: 100%;
}

/* Container Variants */
.form-container--default {
  background: transparent;
}

.form-container--card {
  background: var(--primary-background-color);
  border: 1px solid var(--neutral-color);
  border-radius: 0.75rem;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  padding: 2rem;
}

.form-container--modal {
  background: var(--primary-background-color);
  border-radius: 0.5rem;
  box-shadow: 0 4px 16px rgba(0, 0, 0, 0.15);
  padding: 1.5rem;
}

.form-container--inline {
  background: transparent;
  padding: 0;
}

/* Container Widths */
.form-container--narrow {
  max-width: 400px;
}

.form-container--medium {
  max-width: 600px;
}

.form-container--wide {
  max-width: 800px;
}

.form-container--full {
  max-width: 100%;
}

/* Form Header */
.form-header {
  margin-bottom: 2rem;
  text-align: center;
}

.form-title {
  font-size: 1.75rem;
  font-weight: 600;
  color: var(--primary-text-color);
  margin: 0 0 0.5rem 0;
}

.form-subtitle {
  font-size: 1rem;
  color: var(--tertiary-text-color);
  margin: 0;
  line-height: 1.5;
}

/* Form */
.form {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
  transition: opacity 0.2s ease;
}

.form--loading {
  opacity: 0.7;
  pointer-events: none;
}

.form--disabled {
  opacity: 0.6;
  pointer-events: none;
}

/* Form Sections */
.form-section {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.form-section--before {
  margin-bottom: 0.5rem;
}

.form-section--between {
  margin: 0.5rem 0;
}

.form-section--after {
  margin-top: 0.5rem;
}

/* Form Fields */
.form-fields {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
}

/* Form Actions */
.form-actions {
  display: flex;
  gap: 1rem;
  justify-content: flex-end;
  align-items: center;
  margin-top: 1rem;
  padding-top: 1rem;
  border-top: 1px solid var(--neutral-color);
}

/* Form Footer */
.form-footer {
  margin-top: 1.5rem;
  text-align: center;
  color: var(--tertiary-text-color);
  font-size: 0.875rem;
}

/* Loading Overlay */
.form-loading-overlay {
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(255, 255, 255, 0.8);
  display: flex;
  align-items: center;
  justify-content: center;
  border-radius: inherit;
  z-index: 10;
}

.form-spinner {
  font-size: 1.5rem;
  color: var(--link-color);
}

/* Utility Classes for Form Elements */
.form :deep(.form-field) {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.form :deep(.form-field--horizontal) {
  flex-direction: row;
  align-items: center;
  gap: 1rem;
}

.form :deep(.form-label) {
  font-weight: 500;
  color: var(--primary-text-color);
  font-size: 0.875rem;
}

.form :deep(.form-label--required)::after {
  content: ' *';
  color: var(--error-color);
}

.form :deep(.form-input) {
  padding: 0.75rem;
  border: 1px solid var(--neutral-color);
  border-radius: 0.375rem;
  font-size: 1rem;
  transition: border-color 0.2s ease, box-shadow 0.2s ease;
}

.form :deep(.form-input:focus) {
  outline: none;
  border-color: var(--link-color);
  box-shadow: 0 0 0 0.125rem rgba(0, 123, 255, 0.25);
}

.form :deep(.form-input:invalid) {
  border-color: var(--error-color);
}

.form :deep(.form-error) {
  color: var(--error-color);
  font-size: 0.875rem;
  margin-top: 0.25rem;
}

.form :deep(.form-help) {
  color: var(--tertiary-text-color);
  font-size: 0.875rem;
  margin-top: 0.25rem;
}

.form :deep(.form-textarea) {
  padding: 0.75rem;
  border: 1px solid var(--neutral-color);
  border-radius: 0.375rem;
  font-size: 1rem;
  font-family: inherit;
  resize: vertical;
  min-height: 100px;
  transition: border-color 0.2s ease, box-shadow 0.2s ease;
}

.form :deep(.form-textarea:focus) {
  outline: none;
  border-color: var(--link-color);
  box-shadow: 0 0 0 0.125rem rgba(0, 123, 255, 0.25);
}

.form :deep(.form-file-upload) {
  position: relative;
  display: inline-block;
}

.form :deep(.form-file-input) {
  position: absolute;
  opacity: 0;
  width: 100%;
  height: 100%;
  cursor: pointer;
}

.form :deep(.form-file-button) {
  display: inline-flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.75rem 1rem;
  border: 2px dashed var(--neutral-color);
  border-radius: 0.375rem;
  background: var(--secondary-background-color);
  color: var(--primary-text-color);
  cursor: pointer;
  transition: border-color 0.2s ease, background-color 0.2s ease;
}

.form :deep(.form-file-button:hover) {
  border-color: var(--link-color);
  background: var(--primary-background-color);
}

.form :deep(.form-file-preview) {
  margin-top: 0.5rem;
  display: flex;
  gap: 0.5rem;
  flex-wrap: wrap;
}

.form :deep(.form-file-preview-item) {
  position: relative;
  max-width: 100px;
  max-height: 100px;
  border-radius: 0.375rem;
  overflow: hidden;
  border: 1px solid var(--neutral-color);
}

.form :deep(.form-file-preview-image) {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.form :deep(.form-file-preview-remove) {
  position: absolute;
  top: 0.25rem;
  right: 0.25rem;
  background: rgba(220, 53, 69, 0.8);
  color: white;
  border: none;
  border-radius: 50%;
  width: 1.5rem;
  height: 1.5rem;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  font-size: 0.75rem;
}

.form :deep(.form-tags) {
  display: flex;
  flex-wrap: wrap;
  gap: 0.5rem;
  margin-top: 0.5rem;
}

.form :deep(.form-tag) {
  display: inline-flex;
  align-items: center;
  gap: 0.25rem;
  padding: 0.25rem 0.5rem;
  background: var(--link-color);
  color: white;
  border-radius: 1rem;
  font-size: 0.875rem;
}

.form :deep(.form-tag-remove) {
  background: none;
  border: none;
  color: inherit;
  cursor: pointer;
  padding: 0;
  margin-left: 0.25rem;
  font-size: 0.75rem;
}

.form :deep(.form-button) {
  padding: 0.75rem 1.5rem;
  border: none;
  border-radius: 0.375rem;
  font-size: 1rem;
  font-weight: 500;
  cursor: pointer;
  transition: background-color 0.2s ease, transform 0.1s ease;
}

.form :deep(.form-button:hover) {
  transform: translateY(-1px);
}

.form :deep(.form-button:active) {
  transform: translateY(0);
}

.form :deep(.form-button--primary) {
  background: var(--link-color);
  color: white;
}

.form :deep(.form-button--primary:hover) {
  background: var(--link-color-hover);
}

.form :deep(.form-button--secondary) {
  background: var(--neutral-color);
  color: white;
}

.form :deep(.form-button--secondary:hover) {
  background: var(--tertiary-text-color);
}

.form :deep(.form-button--outline) {
  background: transparent;
  border: 1px solid var(--neutral-color);
  color: var(--primary-text-color);
}

.form :deep(.form-button--outline:hover) {
  background: var(--secondary-background-color);
}

/* Custom styles for PrimeVue components */
:deep(.p-inputtext.p-invalid) {
  border-color: var(--error-color);
}

:deep(.p-inputtextarea.p-invalid) {
  border-color: var(--error-color);
}

/* Responsive Design */
@media (max-width: 768px) {
  .form-container--card,
  .form-container--modal {
    padding: 1rem;
  }
  
  .form-header {
    margin-bottom: 1.5rem;
  }
  
  .form-title {
    font-size: 1.5rem;
  }
  
  .form-actions {
    flex-direction: column-reverse;
    gap: 0.75rem;
  }
  
  .form :deep(.form-button) {
    width: 100%;
  }
}
</style>