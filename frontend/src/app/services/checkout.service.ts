import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { CartService } from './cart.service';
import { Router } from '@angular/router';
@Injectable({
  providedIn: 'root',
})
export class CheckoutService {
  private http = inject(HttpClient);
  private apiUrl = 'https://localhost:7073/api/Checkout';
  private router = inject(Router);
  private cartService = inject(CartService);

  placeOrder(shippingAddress: string) {
    this.http.post(`${this.apiUrl}/PlaceOrder`, { shippingAddress }).subscribe({
      next: () => {
        alert('Order placed successfully!');
        this.cartService.getCart();
        this.cartService.getCartTotal();
        this.router.navigate(['/']);
      },
      error: (err) => {
        console.error('Checkout failed', err);
        alert('Could not place order. Please check your details.');
      },
    });
  }
}
