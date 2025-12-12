<script setup>
import { computed, ref, watch } from 'vue'

const emit = defineEmits(['delete', 'filter-dept'])

const props = defineProps({
  event: {
    type: Object,
    required: true,
  },
  fallback: {
    type: String,
    default: '',
  },
  canDelete: {
    type: Boolean,
    default: false,
  },
})

const currentSrc = ref(props.event.mainImage)

watch(
  () => props.event.mainImage,
  (val) => {
    currentSrc.value = val
  }
)

const formattedDate = computed(() => {
  if (!props.event.eventDate) return ''
  const date = new Date(props.event.eventDate)
  return date.toLocaleDateString('ru-RU')
})
</script>

<template>
  <div class="card">
    <RouterLink :to="`/events/${event.id}`" class="link-area">
      <div class="image-wrap">
        <img
          :src="currentSrc"
          :alt="event.title"
          loading="lazy"
          @error="currentSrc = fallback || event.mainImage"
        />
      </div>
      <div class="content">
        <h3>{{ event.title }}</h3>
    <p class="departments" v-if="event.departments?.length">
      <span
        v-for="dept in event.departments"
        :key="dept.id"
        class="chip"
        @click.stop.prevent="emit('filter-dept', dept.id)"
        title="Показать события отдела"
      >
        {{ dept.name }}
      </span>
    </p>
        <p class="date">{{ formattedDate }}</p>
      </div>
    </RouterLink>
    <button
      v-if="canDelete"
      class="delete-btn"
      type="button"
      @click.stop="emit('delete', event.id)"
      aria-label="Удалить событие"
    >
      ✕
    </button>
  </div>
</template>

<style scoped>
.card {
  position: relative;
  background: var(--card-bg);
  border-radius: var(--radius);
  overflow: hidden;
  box-shadow: var(--shadow-soft);
  display: flex;
  flex-direction: column;
}

.link-area {
  text-decoration: none;
  color: inherit;
  display: flex;
  flex-direction: column;
  height: 100%;
}

.image-wrap {
  width: 100%;
  aspect-ratio: 16 / 9;
  overflow: hidden;
  background: #e6e6e6;
}

.image-wrap img {
  width: 100%;
  height: 100%;
  object-fit: cover;
  display: block;
}

.content {
  padding: 18px 20px 22px;
  display: flex;
  flex-direction: column;
  gap: 10px;
  height: 100%;
}

h3 {
  font-size: 1.1rem;
  line-height: 1.4;
  color: var(--text-primary);
  font-weight: 600;
}

.date {
  color: var(--text-muted);
  font-size: 0.9rem;
}

.departments {
  display: flex;
  flex-wrap: wrap;
  gap: 8px;
}

.chip {
  background: #eef2ff;
  color: #3f51b5;
  border-radius: 999px;
  padding: 6px 12px;
  font-size: 0.85rem;
  font-weight: 600;
  cursor: pointer;
  transition: background 0.15s ease, transform 0.15s ease;
}

.chip:hover {
  background: #dfe5ff;
  transform: translateY(-1px);
}

.delete-btn {
  position: absolute;
  top: 10px;
  right: 10px;
  background: rgba(255, 255, 255, 0.92);
  border: 1px solid #e2e2e2;
  border-radius: 50%;
  width: 34px;
  height: 34px;
  display: grid;
  place-items: center;
  color: #d14343;
  font-size: 16px;
  font-weight: 700;
  cursor: pointer;
  transition: transform 0.15s ease, box-shadow 0.15s ease, background 0.15s ease;
}

.delete-btn:hover {
  background: #fff5f5;
  box-shadow: 0 6px 14px rgba(0, 0, 0, 0.12);
  transform: translateY(-1px);
}
</style>
