import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Cliente } from './cliente.model'; // Assuming the model is defined in cliente.model.ts

@Injectable({
  providedIn: 'root'
})
export class ClienteService {
  private baseUrl: string = 'https://localhost:7012/api/clientes';

  constructor(private http: HttpClient) {}

  loadClientes(userId:number): Observable<Cliente[]> {
    return this.http.get<Cliente[]>(`${this.baseUrl}/${userId}/list`);
  }

  addCliente(cliente: Cliente): Observable<Cliente> {
    return this.http.post<Cliente>(this.baseUrl, cliente);
  }

  editCliente(cliente: Cliente): Observable<Cliente> {
    return this.http.put<Cliente>(`${this.baseUrl}/${cliente.id}`, cliente);
  }

  deleteCliente(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}