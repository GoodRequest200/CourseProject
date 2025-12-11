<script setup>
import { ref } from 'vue'

const firstName = ref('')
const lastName = ref('')
const patronymic = ref('')
const email = ref('')
const password = ref('')
const error = ref('')
const success = ref('')
const loading = ref(false)

const submit = async () => {
  error.value = ''
  success.value = ''

  if (!email.value || !password.value || !firstName.value || !lastName.value) {
    error.value = 'Заполните имя, фамилию, email и пароль'
    return
  }

  loading.value = true
  try {
    const base =
      import.meta.env.VITE_API_BASE_URL ||
      `${window.location.protocol}//${window.location.hostname}:5000`

    const payload = {
      firstName: firstName.value,
      lastName: lastName.value,
      patronymic: patronymic.value || null,
      email: email.value,
      roleId: 2, // роль по умолчанию
      password: password.value,
    }

    const res = await fetch(`${base}/api/users`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(payload),
    })

    if (!res.ok) {
      const text = await res.text()
      throw new Error(text || 'Ошибка регистрации')
    }

    success.value = 'Регистрация прошла успешно'
  } catch (e) {
    error.value = e.message || 'Ошибка регистрации'
  } finally {
    loading.value = false
  }
}
</script>

<template>
  <section class="auth">
    <h1>Регистрация</h1>
    <p class="subtitle">Создайте аккаунт, чтобы управлять событиями</p>
    <form @submit.prevent="submit">
      <label>
        Имя
        <input v-model="firstName" type="text" placeholder="Иван" />
      </label>
      <label>
        Фамилия
        <input v-model="lastName" type="text" placeholder="Иванов" />
      </label>
      <label>
        Отчество (необязательно)
        <input v-model="patronymic" type="text" placeholder="Иванович" />
      </label>
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
        {{ loading ? 'Отправляем...' : 'Зарегистрироваться' }}
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
