<script setup>
import { onMounted, ref, computed } from 'vue'
import { useRoute, useRouter } from 'vue-router'

const route = useRoute()
const router = useRouter()
const event = ref(null)
const loading = ref(true)
const error = ref('')
const defaultImg = ref('')
const comments = ref([])
const commentsError = ref('')
const newComment = ref('')
const posting = ref(false)
const token = ref(localStorage.getItem('token') || '')
const isAdmin = ref(false)

const formattedDate = computed(() => {
  if (!event.value?.eventDate) return ''
  return new Date(event.value.eventDate).toLocaleDateString('ru-RU')
})

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

const userIdFromToken = () => {
  if (!token.value) return null
  try {
    const payload = token.value.split('.')[1]
    const json = JSON.parse(atob(payload.replace(/-/g, '+').replace(/_/g, '/')))
    return json.sub ? Number(json.sub) : null
  } catch {
    return null
  }
}

const baseApi =
  import.meta.env.VITE_API_BASE_URL ||
  `${window.location.protocol}//${window.location.hostname}:5000`

const toAbsolute = (path) => {
  const fallbackPath = defaultImg.value || `${baseApi}/images/default.jpg`
  if (!path) return fallbackPath
  const file = path.split(/[/\\]+/).pop() || 'default.jpg'
  if (/^https?:\/\//i.test(path)) return path
  return `${baseApi}/images/${file}`
}

const loadEvent = async () => {
  loading.value = true
  error.value = ''
  defaultImg.value = toAbsolute('default.jpg')

  try {
    const res = await fetch(`${baseApi}/api/events/${route.params.id}`)
    if (!res.ok) throw new Error('Не удалось загрузить событие')
    const data = await res.json()
    event.value = {
      id: data.id,
      title: data.title,
      content: data.content,
      mainImage: toAbsolute(data.mainImage),
      eventDate: data.eventDate,
      departments: (data.departments ?? []).map((d) => ({ id: d.id, name: d.name })),
    }
  } catch (err) {
    console.error(err)
    error.value = 'Не удалось загрузить событие.'
  } finally {
    loading.value = false
  }
}

const loadComments = async () => {
  commentsError.value = ''
  try {
    const res = await fetch(`${baseApi}/api/comments/event/${route.params.id}`)
    if (!res.ok) throw new Error('Не удалось загрузить комментарии')
    const data = await res.json()
    comments.value = data.map((c) => ({
      id: c.id,
      content: c.content,
      createdAt: c.createdAt,
      userId: c.userId,
    }))
  } catch (e) {
    console.error(e)
    commentsError.value = e.message || 'Не удалось загрузить комментарии'
  }
}

const submitComment = async () => {
  commentsError.value = ''
  const userId = userIdFromToken()
  if (!token.value || !userId) {
    commentsError.value = 'Нужно войти, чтобы оставить комментарий'
    return
  }

  if (!newComment.value.trim()) {
    commentsError.value = 'Комментарий не может быть пустым'
    return
  }

  posting.value = true
  try {
    const payload = {
      eventId: Number(route.params.id),
      userId,
      content: newComment.value.trim(),
    }

    const res = await fetch(`${baseApi}/api/comments`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        Authorization: `Bearer ${token.value}`,
      },
      body: JSON.stringify(payload),
    })

    if (!res.ok) {
      const text = await res.text()
      throw new Error(text || 'Не удалось отправить комментарий')
    }

    const saved = await res.json()
    comments.value.unshift({
      id: saved.id,
      content: saved.content,
      createdAt: saved.createdAt,
      userId: saved.userId,
    })
    newComment.value = ''
  } catch (e) {
    commentsError.value = e.message || 'Не удалось отправить комментарий'
  } finally {
    posting.value = false
  }
}

const deleteComment = async (id) => {
  if (!isAdmin.value) return
  try {
    const res = await fetch(`${baseApi}/api/comments/${id}`, {
      method: 'DELETE',
      headers: { Authorization: `Bearer ${token.value}` },
    })
    if (!res.ok) throw new Error('Не удалось удалить комментарий')
    comments.value = comments.value.filter((c) => c.id !== id)
  } catch (e) {
    commentsError.value = e.message || 'Не удалось удалить комментарий'
  }
}

onMounted(() => {
  parseRole()
  loadEvent()
  loadComments()
})
</script>

<template>
  <section class="page">
    <div v-if="loading" class="state">Загрузка...</div>
    <div v-else-if="error" class="state">{{ error }}</div>
    <div v-else-if="event" class="content">
      <div class="top-bar">
        <p class="date" v-if="formattedDate">{{ formattedDate }}</p>
        <RouterLink v-if="isAdmin" class="btn small" :to="`/events/${event.id}/edit`">
          Редактировать
        </RouterLink>
      </div>
      <h1>{{ event.title }}</h1>
      <div class="departments" v-if="event.departments?.length">
        <span v-for="dept in event.departments" :key="dept.id" class="chip">{{ dept.name }}</span>
      </div>
      <div class="image-wrap">
        <img :src="event.mainImage" :alt="event.title" />
      </div>
      <p class="body" v-if="event.content">{{ event.content }}</p>

      <section class="comments">
        <h2>Комментарии</h2>
        <p v-if="commentsError" class="error">{{ commentsError }}</p>

        <form class="comment-form" @submit.prevent="submitComment" v-if="token">
          <textarea
            v-model="newComment"
            rows="3"
            placeholder="Оставьте комментарий..."
          ></textarea>
          <button type="submit" :disabled="posting">{{ posting ? 'Отправка...' : 'Отправить' }}</button>
        </form>
        <p v-else class="hint">Войдите, чтобы оставить комментарий.</p>

        <div v-if="comments.length" class="comment-list">
          <div v-for="c in comments" :key="c.id" class="comment-item">
            <div class="meta">
              <span class="user">Пользователь #{{ c.userId }}</span>
              <span class="dot">•</span>
              <span class="date">{{ new Date(c.createdAt).toLocaleDateString('ru-RU') }}</span>
              <button v-if="isAdmin" class="link danger" @click="deleteComment(c.id)">Удалить</button>
            </div>
            <p class="text">{{ c.content }}</p>
          </div>
        </div>
        <p v-else class="hint">Комментариев пока нет.</p>
      </section>
    </div>
  </section>
</template>

<style scoped>
.page {
  padding: 48px 28px 64px;
  max-width: 1100px;
  margin: 0 auto;
}

.state {
  text-align: center;
  padding: 40px 0;
  color: #666;
}

.top-bar {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

h1 {
  font-size: clamp(2rem, 3vw, 2.6rem);
  font-weight: 700;
  color: #141414;
  margin: 8px 0 14px;
}

.date {
  color: #8a8a8a;
  font-size: 0.95rem;
  margin-bottom: 6px;
}

.btn.small {
  padding: 8px 12px;
  border: 1px solid #d7d7d7;
  border-radius: 10px;
  text-decoration: none;
  background: #fff;
  color: #333;
}

.departments {
  display: flex;
  gap: 8px;
  flex-wrap: wrap;
  margin-bottom: 18px;
}

.chip {
  background: #eef2ff;
  color: #3f51b5;
  border-radius: 999px;
  padding: 6px 12px;
  font-size: 0.85rem;
  font-weight: 600;
}

.image-wrap {
  width: 100%;
  max-width: 820px;
  margin: 12px auto 22px;
  background: #eaeaea;
  border-radius: 14px;
  overflow: hidden;
  box-shadow: 0 10px 30px rgba(0, 0, 0, 0.08);
}

.image-wrap img {
  width: 100%;
  height: auto;
  display: block;
}

.body {
  font-size: 1.05rem;
  line-height: 1.7;
  color: #2b2b2b;
  white-space: pre-wrap;
}

.comments {
  margin-top: 32px;
  border-top: 1px solid #e7e7e7;
  padding-top: 24px;
}

.comments h2 {
  font-size: 1.4rem;
  margin-bottom: 14px;
}

.comment-form {
  display: flex;
  flex-direction: column;
  gap: 10px;
  margin-bottom: 20px;
}

.comment-form textarea {
  width: 100%;
  padding: 12px;
  border-radius: 12px;
  border: 1px solid #d8d8d8;
  resize: vertical;
  min-height: 80px;
}

.comment-form button {
  align-self: flex-end;
  padding: 10px 16px;
  border: none;
  border-radius: 10px;
  background: #3f51b5;
  color: #fff;
  cursor: pointer;
}

.comment-list {
  display: flex;
  flex-direction: column;
  gap: 14px;
}

.comment-item {
  padding: 12px 14px;
  border-radius: 12px;
  background: #f8f8f8;
}

.meta {
  display: flex;
  align-items: center;
  gap: 8px;
  color: #777;
  font-size: 0.9rem;
  margin-bottom: 6px;
}

.dot {
  font-size: 1rem;
}

.link.danger {
  background: none;
  border: none;
  color: #d14343;
  cursor: pointer;
}

.text {
  color: #1f1f1f;
  white-space: pre-wrap;
}

.error {
  color: #d14343;
}

.hint {
  color: #777;
}
</style>
