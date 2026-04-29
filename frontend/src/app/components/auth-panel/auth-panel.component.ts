import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';

import { LoginRequest } from '../../models/login-request';
import { MeResponse } from '../../models/me-response';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-auth-panel',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './auth-panel.component.html'
})
export class AuthPanelComponent implements OnInit {
  credentials: LoginRequest = {
    username: '',
    password: ''
  };

  session: MeResponse | null = null;
  isCheckingSession = true;
  isSubmitting = false;
  errorMessage = '';
  successMessage = '';

  constructor(private readonly authService: AuthService) {}

  get isAuthenticated(): boolean {
    return this.session?.isAuthenticated === true;
  }

  get rolesLabel(): string {
    return this.session?.roles.length ? this.session.roles.join(', ') : 'Admin';
  }

  ngOnInit(): void {
    this.loadSession();
  }

  login(): void {
    const request: LoginRequest = {
      username: this.credentials.username.trim(),
      password: this.credentials.password
    };

    if (!request.username || !request.password) {
      this.errorMessage = 'Informe usuario e senha.';
      this.successMessage = '';
      return;
    }

    this.isSubmitting = true;
    this.errorMessage = '';
    this.successMessage = '';

    this.authService.login(request).subscribe({
      next: (session) => {
        this.session = session;
        this.credentials.password = '';
        this.successMessage = 'Admin conectado.';
        this.isSubmitting = false;
      },
      error: () => {
        this.credentials.password = '';
        this.errorMessage = 'Usuario ou senha invalidos.';
        this.isSubmitting = false;
      }
    });
  }

  logout(): void {
    this.isSubmitting = true;
    this.errorMessage = '';
    this.successMessage = '';

    this.authService.logout().subscribe({
      next: () => {
        this.session = null;
        this.credentials = {
          username: '',
          password: ''
        };
        this.successMessage = 'Sessao encerrada.';
        this.isSubmitting = false;
      },
      error: () => {
        this.errorMessage = 'Nao foi possivel sair agora.';
        this.isSubmitting = false;
      }
    });
  }

  private loadSession(): void {
    this.authService.me().subscribe({
      next: (session) => {
        this.session = session.isAuthenticated ? session : null;
        this.isCheckingSession = false;
      },
      error: () => {
        this.session = null;
        this.isCheckingSession = false;
      }
    });
  }
}
