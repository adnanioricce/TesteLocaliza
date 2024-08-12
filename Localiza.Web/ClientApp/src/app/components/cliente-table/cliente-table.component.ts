import { HttpClient } from '@angular/common/http';
import { Component, OnChanges, Output, SimpleChanges,EventEmitter, Input } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { Subject } from 'rxjs';
import { AuthService } from 'src/app/services/auth.service';
import { Cliente, ClienteRowViewModel, toView } from 'src/app/services/cliente.model';
import { ClienteService } from 'src/app/services/cliente.service';
import { ICobranca } from 'src/app/services/cobranca.model';
import { CobrancaService } from 'src/app/services/cobranca.service';
interface CreateClienteRequest {
  nome: string
  documento: string
  endereco: string
  telefone: string
}
@Component({
  selector: 'app-cliente-table',
  templateUrl: './cliente-table.component.html',
  styleUrls: ['./cliente-table.component.css']
})
export class ClienteTableComponent {
  // clienteService: ClienteService
  isAddingCliente: boolean = false
  addCobrancaMsg:object = {clienteId: 0}
  initAddCobrancaEvent: Subject<number> = new Subject<number>();
  isAddingCobranca:boolean = false
  clientes:Cliente[] = []
  newCliente: Cliente = { id: 0, usuarioId: 0, documento: '', nome: '',telefone: '',endereco:'',cobrancas:[] };
  clienteForm: FormGroup;
  isEditing = false;
  currentClienteId: number = 0;
  newCobranca: ICobranca = { id: 0, clienteId: 0, valor: 0, dataVencimento: new Date(), pago: false, descricao: '' };
  viewModel: ClienteRowViewModel[] = []
  /**
   *
   */
  constructor(
    private clienteService:ClienteService
    ,private cobrancaService: CobrancaService
    ,private authService: AuthService
    ,private fb: FormBuilder
    ,private router: Router) {
    this.clienteForm = this.fb.group({
      nome: ['',Validators.required],
      documento: ['', Validators.required],
      telefone: ['', Validators.required],
      endereco: ['', Validators.required]
    });    
  }
  public get inputToChild(): object{
    return this.addCobrancaMsg
  }
  public set inputToChild(value:object) {
    this.addCobrancaMsg = value
  }
  ngOnInit(): void {
    this.loadClientes()
  }
  loadClientes(){
    const userId = this.authService.getUserId()
    if(!userId){

      return;
    }
    this.clienteService.loadClientes(userId).subscribe(clientes => {
      this.clientes = (clientes ?? []);
      this.clientes.sort((a,b) => a.id > b.id ? -1 : 1)
      this.viewModel = clientes.map(cl => toView(cl))
      console.log('clientes:',clientes)
    });
  }
  editCliente(cliente:Cliente) {
    if(!this.isEditing){
      console.log('client:',cliente)
      return;
    }
    this.clienteService.editCliente(cliente)
  }

  deleteCliente(clienteId:number) {
    console.log('clienteId:',clienteId)
    this.clienteService.deleteCliente(clienteId).subscribe(_ => {
      this.loadClientes()
    });
  }

  addOrUpdateCliente() {
    const { nome, documento, telefone, endereco }: CreateClienteRequest = this.clienteForm.value
    const userId = this.authService.getUserId()
    const cliente:Cliente = {
      id:0
      ,usuarioId:userId ?? 0
      ,nome
      ,documento
      ,endereco
      ,telefone
      ,cobrancas:[]
    }
    console.log('cliente:',cliente)
    this.clienteService.addCliente(cliente).subscribe(cliente => {
      console.log('newCliente:',cliente)
      this.loadClientes()
    })
    this.isAddingCliente = !this.isAddingCliente
  }
  toggleModal(newState:boolean){
    console.log('newState:',newState)
    this.isAddingCobranca = newState
  }
  // cobranca operations
  openCobrancaModal(clienteId: number): void {
    this.currentClienteId = clienteId;
    this.newCobranca.clienteId = clienteId
    // this.initAddCobrancaEvent.next(clienteId)
    this.isAddingCobranca = true
    // this.onAddCobrancaClick.emit(this.currentClienteId)
    console.log('this.currentClienteId:',this.currentClienteId)

  }

  onCobrancaAdded(cobranca: ICobranca): void {
    console.log('New Cobranca Added:', cobranca);
  }
  openModal(clienteId: number): void {
    if(clienteId === 0){
      return;
    }
    this.newCobranca.clienteId = clienteId;
  }


  closeModal(): void {
    this.currentClienteId = 0
  }

  addCobranca(): void {
    console.log('cobranca:',this.newCobranca)
    this.newCobranca.clienteId = this.currentClienteId
    this.cobrancaService.createCobranca(this.newCobranca).subscribe(cobranca => {
      // this.cobrancaAdded.emit(cobranca);
      this.closeModal();
    });
  }
  goToCobrancas(clienteId:number) {
    this.router.navigate([`/cobrancas/${clienteId}`])
  }
}
