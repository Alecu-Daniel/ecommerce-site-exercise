import { Injectable, signal, computed } from '@angular/core';
import { Product } from '../models/product.model';

@Injectable({
  providedIn: 'root',
})
export class CartService {
  private cartItems = signal<Product[]>([]);

  readonly count = computed(() => this.cartItems().length);

  addToCart(product: Product) {
    this.cartItems.update((currentItems) => [...currentItems, product]);
    console.log('Added to cart:', product.name);
  }

  getCartItems() {
    return this.cartItems();
  }
}
