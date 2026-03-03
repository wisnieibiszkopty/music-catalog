import { createRouter, createWebHistory } from 'vue-router'
import LayoutView from '../views/LayoutView.vue'
import NotFoundView from '../views/NotFoundView.vue'
import keycloak from '@/services/core/keycloak';

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      component: LayoutView,
      children: [
        {
          path: '/artists',
          name: 'Artists',
          component: () => import('../views/ArtistsView.vue'),
        },
        {
          path: '/albums',
          name: 'Albums',
          component: () => import('../views/AlbumsView.vue'),
        },
        {
          path: '/playlists',
          name: 'Playlists',
          component: () => import('../views/PlaylistView.vue'),
          meta: { requiresAuth: true }
        },
        {
          path: '/scrapper',
          name: 'Scrapper',
          component: () => import('../views/ScrapperView.vue'),
          meta: { requiresAuth: true, role: 'admin' }
        }
      ]
    },
    {
      path: '/:pathMatch(.*)*',
      name: 'NotFound',
      component: NotFoundView
    }
  ],
})

router.beforeEach(async (to, from, next) => {
  if(!to.meta.requiresAuth) {
    return next();
  }

  if(!keycloak.authenticated){
    return keycloak.login({
      redirectUri: window.location.origin + to.fullPath
    });
  }

  // TODO not working
  const requiredRole = to.meta.role as string;
  if(requiredRole && !keycloak.hasRealmRole(requiredRole)){
    return next('/');
  }

  next();
})

export default router;
