import { HttpClient } from '@angular/common/http';
import { Injectable, resource, signal } from '@angular/core';
import { tap } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private baseUrl = 'https://localhost:7073/api/Auth';

  currentUserToken = signal<string | null>(localStorage.getItem('token'));

  constructor(private http: HttpClient) {}

  register(model: any) {
    return this.http.post(`${this.baseUrl}/Register`, model);
  }

  login(model: any) {
    return this.http.post<{ token: string }>(`${this.baseUrl}/Login`, model).pipe(
      tap((response) => {
        if (response.token) {
          localStorage.setItem('token', response.token);
          this.currentUserToken.set(response.token);
        }
      }),
    );
  }

  logout() {
    localStorage.removeItem('token');
    this.currentUserToken.set(null);
  }
}
