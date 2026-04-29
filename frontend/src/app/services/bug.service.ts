import { BugResponse } from './../models/bug-response';
import { CriarBugRequest } from './../models/criar-bug-request';
import { Injectable, inject} from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_URL } from '../config/api.config';


@Injectable({ providedIn: 'root' })
export class BugService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${API_URL}/bugs`;

  listar(status?: string, linguagem?: string): Observable<BugResponse[]> {
    let params = new HttpParams();

    if (status) {
      params = params.set('status', status);
    }

    if (linguagem) {
      params = params.set('linguagem', linguagem);
    }

    return this.http.get<BugResponse[]>(this.baseUrl, { params });
  }
  buscarPorId(id: number): Observable<BugResponse> {
    return this.http.get<BugResponse>(`${this.baseUrl}/${id}`);
  }
  criar(request: CriarBugRequest): Observable<BugResponse>{
    return this.http.post<BugResponse>(
      this.baseUrl,
      request,
      {withCredentials: true}
    )
  }
}
