import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ICobranca } from './cobranca.model';

@Injectable({
  providedIn: 'root'
})
export class CobrancaService {
  private baseUrl: string = 'http://localhost:5047/api/cobrancas';

  constructor(private http: HttpClient) {}

  createCobranca(cobranca: ICobranca): Observable<ICobranca> {
    return this.http.post<ICobranca>(this.baseUrl, cobranca);
  }

  getCobrancaById(id: number): Observable<ICobranca> {
    return this.http.get<ICobranca>(`${this.baseUrl}/${id}`);
  }

  getAllCobrancas(clienteId: number): Observable<ICobranca[]> {
    return this.http.get<ICobranca[]>(`${this.baseUrl.replace('cobrancas','clientes')}/${clienteId}/cobrancas`);
  }

  updateCobranca(cobranca: ICobranca): Observable<ICobranca> {
    return this.http.put<ICobranca>(`${this.baseUrl}/${cobranca.id}`, cobranca);
  }

  deleteCobranca(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
