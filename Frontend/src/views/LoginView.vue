<script setup>
import { ref } from 'vue'

const email = ref('')
const password = ref('')
const error = ref('')
const success = ref('')
const loading = ref(false)

const submit = async () => {
  error.value = ''
  success.value = ''

  if (!email.value || !password.value) {
    error.value = 'Введите email и пароль'
    return
  }

  loading.value = true
  try {
    const base =
      import.meta.env.VITE_API_BASE_URL ||
      `${window.location.protocol}//${window.location.hostname}:5000`

    const res = await fetch(`${base}/api/auth/login`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ email: email.value, password: password.value }),
    })

    if (!res.ok) throw new Error('Неверные данные для входа')
    const data = await res.json()
    localStorage.setItem('token', data.token)
    success.value = 'Вход выполнен успешно'
  } catch (e) {
    error.value = e.message || 'Ошибка авторизации'
  } finally {
    loading.value = false
  }
}
</script>

<template>
  <section class="auth">
    <h1>Вход</h1>
    <p class="subtitle">Авторизуйтесь, чтобы управлять событиями</p>
    <form @submit.prevent="submit">
      <label>
        Email
        <input v-model="email" type="email" placeholder="you@example.com" />
      </label>
      <label>
        Пароль
        <input v-model="password" type="password" placeholder="••••••••" />
      </label>
      <p v-if="error" class="error">{{ error }}</p>
      <p v-if="success" class="success">{{ success }}</p>
      <button type="submit" :disabled="loading">
        {{ loading ? 'Входим...' : 'Войти' }}
      </button>
    </form>
  </section>
</template>

<style scoped>
.auth {
  max-width: 420px;
  margin: 0 auto;
  padding: 48px 24px 64px;
}

h1 {
  font-size: 2rem;
  margin-bottom: 8px;
}

.subtitle {
  color: #666;
  margin-bottom: 24px;
}

form {
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

input {
  padding: 10px 12px;
  border: 1px solid #d8d8d8;
  border-radius: 10px;
  font-size: 1rem;
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
}

button:hover {
  opacity: 0.92;
}

button:active {
  transform: translateY(1px);
}

.error {
  color: #d14343;
}

.success {
  color: #2c8f4f;
}
</style>
