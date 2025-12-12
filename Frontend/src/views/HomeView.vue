<script setup>
import { onMounted, ref } from 'vue'
import EventCard from '../components/EventCard.vue'
import { RouterLink } from 'vue-router'

const events = ref([])
const filtered = ref([])
const departments = ref([])
const selectedDept = ref('')
const searchTerm = ref('')
const dateFrom = ref('')
const dateTo = ref('')
const loading = ref(true)
const error = ref('')
const defaultImg = ref('')
const token = ref(localStorage.getItem('token') || '')
const isAdmin = ref(false)

const apiBase = () =>
  import.meta.env.VITE_API_BASE_URL ||
  `${window.location.protocol}//${window.location.hostname}:5000`

const fallback = [
  {
    id: 1,
    title: 'Уже готовимся к гастрольному туру Unity Songs',
    mainImage: 'default.jpg',
    eventDate: '2025-11-02',
    departments: [{ id: 1, name: 'PR' }],
  },
  {
    id: 2,
    title: 'Музыкальный марафон «Дни Победы»',
    mainImage: 'default.jpg',
    eventDate: '2025-09-02',
    departments: [{ id: 2, name: 'Culture' }],
  },
  {
    id: 3,
    title: 'Форум-фестиваль «Территория будущего Москва 2030»',
    mainImage: 'default.jpg',
    eventDate: '2025-08-11',
    departments: [],
  },
]

const mapEvents = (items, toAbsolute) =>
  items.map((e) => ({
    id: e.id,
    title: e.title,
    mainImage: toAbsolute(e.mainImage ?? 'default.jpg'),
    eventDate: e.eventDate,
    departments: (e.departments ?? []).map((d) => ({ id: d.id, name: d.name })),
  }))

const loadEvents = async () => {
  loading.value = true
  error.value = ''

  const base = apiBase()

const toAbsolute = (path) => {
  const fallbackPath = defaultImg.value || `${base}/images/default.jpg`
  if (!path) return fallbackPath

  const file = path.split(/[/\\]+/).pop() || 'default.jpg'
  if (/^https?:\/\//i.test(path)) return path
  return `${base}/images/${file}`
}

  defaultImg.value = toAbsolute('default.jpg')

  try {
    const res = await fetch(`${base}/api/events`)

    if (!res.ok)
      throw new Error('Не удалось загрузить события')

    const data = await res.json()
    events.value = mapEvents(data, toAbsolute)

    const set = new Map()

    events.value.forEach((ev) => (ev.departments || []).forEach((d) => set.set(d.id, d.name)))

    departments.value = Array.from(set, ([id, name]) => ({ id, name }))
  } catch (err) {
    console.error(err)
    error.value = 'Не удалось загрузить события, показаны заглушки.'

    events.value = mapEvents(fallback, toAbsolute)

    departments.value = []
  } finally {
    applyFilter()
    loading.value = false
  }
}

const normalizeDate = (value) => {
  if (!value) return null
  const date = new Date(value)
  return isNaN(date.getTime()) ? null : date
}

const applyFilter = () => {
  const deptId = selectedDept.value ? Number(selectedDept.value) : null
  const query = searchTerm.value.trim().toLowerCase()
  const from = normalizeDate(dateFrom.value)
  const to = normalizeDate(dateTo.value)

  filtered.value = events.value.filter((ev) => {
    const matchesDept =
      !deptId || (ev.departments || []).some((d) => d.id === deptId)
    const matchesTitle =
      !query || (ev.title || '').toLowerCase().includes(query)

    const evDate = normalizeDate(ev.eventDate)
    const matchesFrom = !from || (evDate && evDate >= from)
    const matchesTo = !to || (evDate && evDate <= to)
    const matchesDate = matchesFrom && matchesTo

    return matchesDept && matchesTitle && matchesDate
  })
}

onMounted(loadEvents)

const parseRole = () => {
  if (!token.value) {
    isAdmin.value = false
    return
  }
  try {
    const payload = token.value.split('.')[1]
    const json = JSON.parse(atob(payload.replace(/-/g, '+').replace(/_/g, '/')))
    const role = json?.role || json?.['http://schemas.microsoft.com/ws/2008/06/identity/claims/role']
    isAdmin.value = role === 'administrator'
  } catch (e) {
    isAdmin.value = false
  }
}

parseRole()

const handleDelete = async (id) => {
  if (!isAdmin.value || !id) return
  const confirmDelete = window.confirm('Удалить событие?')
  if (!confirmDelete) return

  try {
    const res = await fetch(`${apiBase()}/api/events/${id}`, {
      method: 'DELETE',
      headers: {
        Authorization: `Bearer ${token.value || ''}`,
      },
    })

    if (!res.ok) throw new Error('Не удалось удалить событие')

    events.value = events.value.filter((ev) => ev.id !== id)
    applyFilter()
  } catch (err) {
    console.error(err)
    alert('Ошибка при удалении события')
  }
}

const handleDeptClick = (deptId) => {
  selectedDept.value = String(deptId)
  applyFilter()
}
</script>

<template>
  <section class="page">
    <header class="hero">
      <p class="eyebrow">Ансамбль песни и пляски Сиверко</p>
      <h1>Новости и мероприятия</h1>
      <p class="subtitle">Следите за свежими событиями и объявлениями</p>
      <div class="auth-actions">
        <RouterLink class="btn ghost" to="/login">Войти</RouterLink>
        <RouterLink class="btn primary" to="/register">Регистрация</RouterLink>
        <RouterLink v-if="isAdmin" class="btn primary" to="/events/create">Добавить событие</RouterLink>
      </div>
    </header>

    <div v-if="loading" class="state">Загрузка...</div>
    <div v-else-if="!events.length" class="state">Пока нет событий</div>
    <div v-else>
      <div class="filters">
        <label class="field">
          <span class="label">Поиск</span>
          <input
            type="search"
            v-model="searchTerm"
            @input="applyFilter"
            placeholder="Найти мероприятие"
          />
        </label>
        <div class="date-range">
          <label class="field">
            <span class="label">С</span>
            <input type="date" v-model="dateFrom" @change="applyFilter" />
          </label>
          <label class="field">
            <span class="label">По</span>
            <input type="date" v-model="dateTo" @change="applyFilter" />
          </label>
        </div>
        <label class="field" v-if="departments.length">
          <span class="label">Отдел</span>
          <select v-model="selectedDept" @change="applyFilter">
            <option value="">Все</option>
            <option v-for="d in departments" :key="d.id" :value="d.id">{{ d.name }}</option>
          </select>
        </label>
      </div>
      <div class="grid">
        <EventCard
          v-for="e in filtered"
          :key="e.id"
          :event="e"
          :fallback="defaultImg"
          :can-delete="isAdmin"
          @delete="handleDelete"
          @filter-dept="handleDeptClick"
        />
      </div>
    </div>

    <p v-if="error" class="hint">{{ error }}</p>
  </section>
</template>

<style scoped>
.page {
  padding: 48px 28px 64px;
  max-width: 1280px;
  margin: 0 auto;
}

.hero {
  text-align: center;
  margin-bottom: 36px;
}

.eyebrow {
  text-transform: uppercase;
  letter-spacing: 0.12em;
  font-size: 0.85rem;
  color: #777;
  margin-bottom: 8px;
}

h1 {
  font-size: clamp(2rem, 3vw, 2.6rem);
  font-weight: 700;
  color: #141414;
  margin-bottom: 8px;
}

.subtitle {
  color: #666;
  font-size: 1rem;
}

.auth-actions {
  display: flex;
  justify-content: center;
  gap: 12px;
  margin-top: 18px;
}

.btn {
  padding: 10px 16px;
  border-radius: 12px;
  border: 1px solid #d7d7d7;
  text-decoration: none;
  font-weight: 600;
  transition: 0.2s ease;
}

.btn.ghost {
  background: #fff;
  color: #2b2b2b;
}

.btn.primary {
  background: #3f51b5;
  color: #fff;
  border-color: #3f51b5;
}

.btn:hover {
  transform: translateY(-1px);
  box-shadow: 0 8px 20px rgba(0, 0, 0, 0.08);
}

.filters {
  margin: 12px 0 16px;
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(220px, 1fr));
  gap: 16px;
  align-items: end;
}

.field {
  display: flex;
  flex-direction: column;
  gap: 6px;
}

.label {
  color: #555;
  font-weight: 600;
  font-size: 0.95rem;
}

.field input,
.field select {
  padding: 10px 12px;
  border-radius: 10px;
  border: 1px solid #d8d8d8;
  background: #fff;
  font-size: 0.95rem;
}

.field input:focus,
.field select:focus {
  outline: none;
  border-color: #3f51b5;
  box-shadow: 0 0 0 3px rgba(63, 81, 181, 0.12);
}

.date-range {
  display: grid;
  grid-template-columns: repeat(2, minmax(0, 1fr));
  gap: 10px;
}

.grid {
  display: grid;
  grid-template-columns: repeat(3, minmax(0, 1fr));
  gap: 28px;
  margin-top: 24px;
}

@media (max-width: 1100px) {
  .grid {
    grid-template-columns: repeat(2, minmax(0, 1fr));
  }
}

@media (max-width: 720px) {
  .grid {
    grid-template-columns: 1fr;
  }
}

.state {
  text-align: center;
  color: #666;
  padding: 40px 0;
}

.hint {
  margin-top: 16px;
  text-align: center;
  color: #a45b00;
}
</style>
