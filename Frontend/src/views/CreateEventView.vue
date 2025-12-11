<script setup>
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'

const router = useRouter()
const title = ref('')
const content = ref('')
const eventDate = ref('')
const isCompleted = ref(false)
const departments = ref([])
const selectedDepartments = ref([])
const mainImage = ref(null)
const error = ref('')
const success = ref('')
const loading = ref(false)
const token = ref(localStorage.getItem('token') || '')
const isAdmin = ref(false)

const baseApi =
  import.meta.env.VITE_API_BASE_URL ||
  `${window.location.protocol}//${window.location.hostname}:5000`

const parseRole = () => {
  if (!token.value) {
    isAdmin.value = false
    return
  }
  try {
    const payload = token.value.split('.')[1]
    const json = JSON.parse(atob(payload.replace(/-/g, '+').replace(/_/g, '/')))
    const role =
      json?.role || json?.['http://schemas.microsoft.com/ws/2008/06/identity/claims/role']
    isAdmin.value = role === 'administrator'
  } catch (e) {
    isAdmin.value = false
  }
}

parseRole()

const loadDepartments = async () => {
  try {
    const res = await fetch(`${baseApi}/api/departments`)
    if (!res.ok) throw new Error('Не удалось загрузить департаменты')
    const data = await res.json()
    departments.value = data
  } catch (e) {
    console.error(e)
  }
}

onMounted(loadDepartments)
onMounted(() => {
  if (!isAdmin.value) {
    error.value = 'У вас нет прав для создания событий'
  }
})

const handleFile = (e) => {
  mainImage.value = e.target.files?.[0] || null
}

const submit = async () => {
  error.value = ''
  success.value = ''

  if (!isAdmin.value) {
    error.value = 'Только администратор может создавать события'
    return
  }

  if (!title.value.trim()) {
    error.value = 'Введите заголовок'
    return
  }

loading.value = true
  try {
    const form = new FormData()
    form.append('Title', title.value)
    form.append('Content', content.value || '')
    form.append('IsCompleted', String(isCompleted.value))
    if (eventDate.value) form.append('EventDate', eventDate.value)
    selectedDepartments.value.forEach((id) => form.append('DepartmentIds', id))
    if (mainImage.value) form.append('MainImage', mainImage.value)

    const res = await fetch(`${baseApi}/api/events`, {
      method: 'POST',
      headers: {
        Authorization: `Bearer ${token.value}`,
      },
      body: form,
    })

    if (!res.ok) {
      const text = await res.text()
      throw new Error(text || 'Не удалось создать событие')
    }

    const created = await res.json()
    success.value = 'Событие создано'
    router.push(`/events/${created.id}`)
  } catch (e) {
    error.value = e.message || 'Не удалось создать событие'
  } finally {
    loading.value = false
  }
}
</script>

<template>
  <section class="page">
    <h1>Создать событие</h1>
    <p class="subtitle">Доступно только администраторам</p>
    <p v-if="!isAdmin" class="error">У вас нет прав для создания событий</p>

    <form v-if="isAdmin" class="form" @submit.prevent="submit">
      <label>
        Заголовок
        <input v-model="title" type="text" placeholder="Название события" />
      </label>

      <label>
        Дата
        <input v-model="eventDate" type="date" />
      </label>

      <label class="checkbox">
        <input type="checkbox" v-model="isCompleted" />
        Завершённое событие
      </label>

      <label>
        Описание
        <textarea v-model="content" rows="6" placeholder="Текст статьи"></textarea>
      </label>

      <label>
        Цеха
        <div class="chips">
          <label v-for="d in departments" :key="d.id" class="chip-checkbox">
            <input
              type="checkbox"
              :value="d.id"
              v-model="selectedDepartments"
            />
            <span>{{ d.name }}</span>
          </label>
        </div>
      </label>

      <label>
        Картинка
        <input type="file" accept="image/*" @change="handleFile" />
      </label>

      <p v-if="error" class="error">{{ error }}</p>
      <p v-if="success" class="success">{{ success }}</p>
      <button type="submit" :disabled="loading">
        {{ loading ? 'Создаём...' : 'Создать' }}
      </button>
    </form>
  </section>
</template>

<style scoped>
.page {
  max-width: 900px;
  margin: 0 auto;
  padding: 48px 24px 64px;
}

h1 {
  font-size: 2rem;
  margin-bottom: 6px;
}

.subtitle {
  color: #666;
  margin-bottom: 16px;
}

.form {
  display: flex;
  flex-direction: column;
  gap: 16px;
}

label {
  display: flex;
  flex-direction: column;
  gap: 6px;
  font-weight: 600;
  color: #1c1c1c;
}

input[type='text'],
input[type='date'],
textarea,
input[type='file'] {
  padding: 10px 12px;
  border: 1px solid #d8d8d8;
  border-radius: 10px;
  font-size: 1rem;
}

textarea {
  min-height: 120px;
  resize: vertical;
}

.checkbox {
  flex-direction: row;
  align-items: center;
  gap: 10px;
  font-weight: 500;
}

.chips {
  display: flex;
  flex-wrap: wrap;
  gap: 10px;
}

.chip-checkbox {
  display: inline-flex;
  align-items: center;
  gap: 6px;
  padding: 8px 10px;
  border: 1px solid #d8d8d8;
  border-radius: 12px;
  background: #f9f9f9;
  cursor: pointer;
  font-weight: 500;
}

button {
  padding: 12px 14px;
  border: none;
  border-radius: 12px;
  background: #3f51b5;
  color: #fff;
  font-size: 1rem;
  cursor: pointer;
  transition: transform 0.1s ease, opacity 0.2s ease;
  align-self: flex-start;
}

button:hover {
  opacity: 0.92;
}

button:active {
  transform: translateY(1px);
}

button:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.error {
  color: #d14343;
}

.success {
  color: #2c8f4f;
}
</style>
