import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { API_URL } from '../config/api.config';
import { LoginRequest } from '../models/login-request';
import { MeResponse } from '../models/me-response';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${API_URL}/auth`;

  login(request: LoginRequest): Observable<MeResponse> {
    return this.http.post<MeResponse>(
      `${this.baseUrl}/login`,
      request,
      { withCredentials: true }
    );
  }

  logout(): Observable<void> {
    return this.http.post<void>(
      `${this.baseUrl}/logout`,
      {},
      { withCredentials: true }
    );
  }

  me(): Observable<MeResponse> {
    return this.http.get<MeResponse>(
      `${this.baseUrl}/me`,
      { withCredentials: true }
    );
  }
}
