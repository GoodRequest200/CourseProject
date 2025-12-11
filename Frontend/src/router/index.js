import { createRouter, createWebHistory } from 'vue-router'
import HomeView from '../views/HomeView.vue'
const EventDetailView = () => import('../views/EventDetailView.vue')
const LoginView = () => import('../views/LoginView.vue')
const RegisterView = () => import('../views/RegisterView.vue')
const CreateEventView = () => import('../views/CreateEventView.vue')
const EditEventView = () => import('../views/EditEventView.vue')

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'home',
      component: HomeView,
    },
    {
      path: '/about',
      name: 'about',
      // route level code-splitting
      // this generates a separate chunk (About.[hash].js) for this route
      // which is lazy-loaded when the route is visited.
      component: () => import('../views/AboutView.vue'),
    },
    {
      path: '/events/:id',
      name: 'event-detail',
      component: EventDetailView,
      props: true,
    },
    {
      path: '/events/create',
      name: 'event-create',
      component: CreateEventView,
    },
    {
      path: '/events/:id/edit',
      name: 'event-edit',
      component: EditEventView,
      props: true,
    },
    {
      path: '/login',
      name: 'login',
      component: LoginView,
    },
    {
      path: '/register',
      name: 'register',
      component: RegisterView,
    },
  ],
})

export default router
