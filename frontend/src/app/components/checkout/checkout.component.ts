import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { CheckoutService } from '../../services/checkout.service';
import { CartService } from '../../services/cart.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-checkout',
  imports: [ReactiveFormsModule, CommonModule],
  templateUrl: './checkout.component.html',
  styleUrl: './checkout.component.css',
})
export class CheckoutComponent {
  private fb = inject(FormBuilder);
  private checkoutService = inject(CheckoutService);
  private cartService = inject(CartService);

  total = this.cartService.cartTotal;

  checkoutForm = this.fb.group({
    shippingAddress: ['', [Validators.required]],
  });

  onSubmit() {
    if (this.checkoutForm.valid) {
      const address = this.checkoutForm.value.shippingAddress!;
      this.checkoutService.placeOrder(address);
    }
  }
}
