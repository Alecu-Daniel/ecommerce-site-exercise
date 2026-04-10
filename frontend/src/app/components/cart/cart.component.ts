import { Component, computed, inject } from '@angular/core';
import { CartService } from '../../services/cart.service';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-cart',
  imports: [CommonModule, RouterLink],
  templateUrl: './cart.component.html',
  styleUrl: './cart.component.css',
})
export class CartComponent {
  private cartService = inject(CartService);

  cartItems = this.cartService.cartItems;
  cartTotal = this.cartService.cartTotal;

  ngOnInit() {
    this.cartService.getCart();
    this.cartService.getCartTotal();
  }

  onUpdateQuantity(productId: number, quantity: number) {
    this.cartService.updateQuantity(productId, quantity);
  }

  onRemove(productId: number) {
    this.cartService.removeItem(productId);
  }
}
