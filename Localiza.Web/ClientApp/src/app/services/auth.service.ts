import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Router } from '@angular/router';

export interface LoginResponse {
  token: string;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private baseUrl: string = 'http://localhost:5047/api/auth'; // Adjust to your API endpoint
  private token: string | null = null;

  constructor(private http: HttpClient, private router: Router) {}

  login(username: string, password: string): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${this.baseUrl}/login`, { username, password });
  }

  logout(): void {
    this.token = null;
    localStorage.removeItem('authToken');
    this.router.navigate(['/login']); // Redirect to login page or appropriate route
  }

  setToken(token: string): void {
    this.token = token;
    localStorage.setItem('authToken', token);
  }
  getUserId(): number | null {
    const token = this.getToken()
    if(!token){
      return null
    }
    const decodedJwt = this.parseJwt(token)
    const userId = parseInt(decodedJwt.sub)
    return userId
  }
  getToken(): string | null {
    if (!this.token) {
      this.token = localStorage.getItem('authToken');
    }
    return this.token;
  }

  isAuthenticated(): boolean {
    return !!this.getToken(); // Returns true if token exists
  }
  parseJwt(token:string) {
    try {
        const base64Url = token.split('.')[1];
        const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
        const jsonPayload = decodeURIComponent(atob(base64).split('').map(function(c) {
            return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
        }).join(''));

        return JSON.parse(jsonPayload);
    } catch (e) {
        console.error('Invalid token', e);
        return null;
    }
}
}