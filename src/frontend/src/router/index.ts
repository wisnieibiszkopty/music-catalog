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
          path: '/artists/:id',
          name: 'ArtistsDetails',
          component: () => import('../views/ArtistDetailsView.vue'),
          props: true
        },
        {
          path: '/albums/:id',
          name: 'Albums',
          component: () => import('../views/AlbumView.vue'),
          props: true
        },
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

  const requiredRole = to.meta.role as string;
  if(requiredRole && !keycloak.isAdmin()){
    return next('/');
  }

  next();
})

export default router;
