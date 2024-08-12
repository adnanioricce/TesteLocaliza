import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Cobranca } from './cobranca.model';

@Injectable({
  providedIn: 'root'
})
export class CobrancaService {
  private baseUrl: string = 'https://localhost:7012/api/cobrancas';

  constructor(private http: HttpClient) {}

  createCobranca(cobranca: Cobranca): Observable<Cobranca> {
    return this.http.post<Cobranca>(this.baseUrl, cobranca);
  }

  getCobrancaById(id: number): Observable<Cobranca> {
    return this.http.get<Cobranca>(`${this.baseUrl}/${id}`);
  }

  getAllCobrancas(clienteId: number): Observable<Cobranca[]> {
    return this.http.get<Cobranca[]>(`${this.baseUrl.replace('cobrancas','clientes')}/${clienteId}/cobrancas`);
  }

  updateCobranca(cobranca: Cobranca): Observable<Cobranca> {
    return this.http.put<Cobranca>(`${this.baseUrl}/${cobranca.id}`, cobranca);
  }

  deleteCobranca(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}