import { Routes } from '@angular/router';
import { ProductListComponent } from './components/product-list/product-list';

export const routes: Routes = [
  { path: '', component: ProductListComponent },
  { path: 'products', component: ProductListComponent },
];
