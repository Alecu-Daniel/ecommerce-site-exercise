import { Injectable, signal, computed, inject, effect } from '@angular/core';
import { Product } from '../models/product.model';
import { HttpClient } from '@angular/common/http';
import { AuthService } from './auth.service';
import { CartItem } from '../models/cart-item.model';
import { CartTotalCost } from '../models/cart-total-cost.model';

@Injectable({
  providedIn: 'root',
})
export class CartService {
  private http = inject(HttpClient);
  private authService = inject(AuthService);
  apiUrl = 'https://localhost:7073/api/Cart';

  constructor() {
    effect(() => {
      if (this.authService.currentUserToken()) {
        this.getCart();
        this.getCartTotal();
      } else {
        this.cartItems.set([]);
      }
    });
  }

  public cartItems = signal<CartItem[]>([]);
  public cartTotal = signal<CartTotalCost>({ total: 0 });

  readonly count = computed(() =>
    this.cartItems().reduce((acc: number, item: CartItem) => acc + item.quantity, 0),
  );

  getCart() {
    this.http.get<CartItem[]>(`${this.apiUrl}/GetCart`).subscribe((items) => {
      this.cartItems.set(items);
    });
  }

  addToCart(productId: number, quantity: number = 1) {
    this.http.post(`${this.apiUrl}/AddToCart`, { productId, quantity }).subscribe(() => {
      this.getCart();
      this.getCartTotal();
      alert('Added to cart!');
    });
  }

  clearCart() {
    this.cartItems.set([]);
  }

  getCartTotal() {
    this.http.get<CartTotalCost>(`${this.apiUrl}/GetTotal`).subscribe((total) => {
      this.cartTotal.set(total);
    });
  }

  removeItem(productId: number) {
    this.http.delete(`${this.apiUrl}/RemoveItem/${productId}`).subscribe({
      next: () => {
        this.getCartTotal();
        this.getCart();
      },
      error: (err) => console.error('Delete failed', err),
    });
  }

  updateQuantity(productId: number, quantity: number) {
    this.http.post(`${this.apiUrl}/AddToCart`, { productId, quantity }).subscribe({
      next: () => {
        this.getCart();
        this.getCartTotal();
      },
      error: (err) => console.error('Update failed', err),
    });
  }
}
