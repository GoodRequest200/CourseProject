<script setup>
import { computed, ref, watch } from 'vue'

const props = defineProps({
  event: {
    type: Object,
    required: true,
  },
  fallback: {
    type: String,
    default: '',
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
<RouterLink :to="`/events/${event.id}`" class="card">
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
      <span v-for="dept in event.departments" :key="dept.id" class="chip">{{ dept.name }}</span>
    </p>
    <p class="date">{{ formattedDate }}</p>
  </div>
</RouterLink>
</template>

<style scoped>
.card {
  background: var(--card-bg);
  border-radius: var(--radius);
  overflow: hidden;
  box-shadow: var(--shadow-soft);
  display: flex;
  flex-direction: column;
  text-decoration: none;
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
}
</style>
