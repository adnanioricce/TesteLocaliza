import { Component, EventEmitter, Output,Input } from '@angular/core'
import { ICobranca } from '../../services/cobranca.model'
import { CobrancaService } from '../../services/cobranca.service'
import { Observable, Subscription } from 'rxjs';

@Component({
  selector: 'app-cobranca-modal',
  templateUrl: './app-cobranca-modal.component.html',
  styleUrls: ['./app-cobranca-modal.component.css']
})
export class CreateEditModalComponent {
  private eventsSubscription: Subscription | null = null;
  @Output() cobrancaAdded = new EventEmitter<ICobranca>();
  @Output() modalStateChange = new EventEmitter<boolean>();
  @Input() addCobrancaEvent: Observable<number> | null = null;
  @Input() addCobrancaMsg: object = { clienteId: 0}
  _clienteId: number = 0;
  newCobranca: ICobranca = { id: 0, clienteId: 0, valor: 0, dataVencimento: new Date(), pago: false, descricao: '' };
  // showModal: boolean = false;

  constructor(private cobrancaService: CobrancaService) {

  }
  ngOnInit(){
    if(!this.addCobrancaEvent){
      console.log('no events')
      return;
    }
    this.eventsSubscription = this.addCobrancaEvent.subscribe((id) => {
      console.log('id:',id)
      // this.openModal(id)
    });
  }

  ngOnDestroy() {
    if(!this.eventsSubscription){
      return;
    }
    this.eventsSubscription.unsubscribe();
  }

  public get clienteId() : number {
    return this._clienteId
  }
  @Input()
  public set clienteId(id:number) {
    this._clienteId = id
    // this.openModal(id);
  }
  openModal(clienteId: number): void {
    if(clienteId === 0){
      return;
    }
    this.newCobranca.clienteId = clienteId;
    console.log('this.eventsSubscription:',this.eventsSubscription)
    // this.showModal = true;
    this.modalStateChange.emit(true)
  }


  closeModal(): void {
    this.clienteId = 0
    this.modalStateChange.emit(false)
    // this.showModal = false;
  }

  addCobranca(): void {
    this.cobrancaService.createCobranca(this.newCobranca).subscribe(cobranca => {
      this.cobrancaAdded.emit(cobranca);
      this.closeModal();
    });
  }
}
