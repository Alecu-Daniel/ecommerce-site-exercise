import { Component, OnInit, signal, inject, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { Product } from '../../models/product.model';
import { ProductService } from '../../services/product.service';
import { CommonModule } from '@angular/common';
import { CartService } from '../../services/cart.service';
import { Router, RouterLink } from '@angular/router';

@Component({
  selector: 'app-product-list',
  imports: [CommonModule, RouterLink],
  templateUrl: './product-list.component.html',
  styleUrl: './product-list.component.css',
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
})
export class ProductListComponent implements OnInit {
  private router = inject(Router);
  private productService = inject(ProductService);
  private cartService = inject(CartService);

  products = signal<Product[]>([]);
  showPopup = signal(false);

  ngOnInit(): void {
    this.productService.getProducts().subscribe({
      next: (data) => {
        this.products.set(data);
      },
      error: (err) => console.log('Could not fetch products', err),
    });
  }

  onAddToCart(product: Product) {
    if (!localStorage.getItem('token')) {
      this.router.navigate(['/login']);
      return;
    }

    this.cartService.addToCart(product.productId);
    this.showPopup.set(true);
  }

  closePopup() {
    this.showPopup.set(false);
  }
}
