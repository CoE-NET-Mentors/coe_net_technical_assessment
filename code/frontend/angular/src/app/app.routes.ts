import { Routes } from '@angular/router';
import { HomeComponent } from './pages/home/home.component';
import { PageNotFoundComponent } from './pages/page-not-found/page-not-found.component';
import { AboutComponent } from './pages/about/about.component';
import { OrdersComponent } from './pages/orders/orders.component';
import { ProductsComponent } from './pages/products/products.component';
import { SupportTicketsComponent } from './pages/support-tickets/support-tickets.component';

export const routes: Routes = [
    { path: 'home', component: HomeComponent  },
    { path: 'characters/:id', loadComponent: () => import('./pages/character-detail/character-detail.component').then(m => m.CharacterDetailComponent) },
    { path: 'characters/:id/edit', loadComponent: () => import('./pages/character-edit/character-edit.component').then(m => m.CharacterEditComponent) },
    { path: 'about', component: AboutComponent  },
    { path: 'orders', component: OrdersComponent },
    { path: 'products', component: ProductsComponent },
    { path: 'support-tickets', component: SupportTicketsComponent },
    { path: '', redirectTo: '/home', pathMatch: 'full' },
    { path: '**', component: PageNotFoundComponent }
];
