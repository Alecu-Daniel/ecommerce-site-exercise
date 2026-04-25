import { Component, inject, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { RouterLink } from '@angular/router';
import { CartService } from '../../services/cart.service';

import { AuthService } from '../../services/auth.service';
@Component({
  selector: 'app-header',
  imports: [RouterLink],
  templateUrl: './header.component.html',
  styleUrl: './header.component.css',
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
})
export class HeaderComponent {
  private cartService = inject(CartService);
  public authService = inject(AuthService);

  logout() {
    this.authService.logout();
  }
  cartCount = this.cartService.count;
}
