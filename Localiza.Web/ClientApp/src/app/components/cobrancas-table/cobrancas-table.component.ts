import { Component, OnInit } from '@angular/core';
import { CobrancaService } from 'src/app/services/cobranca.service';
import { ICobranca } from 'src/app/services/cobranca.model';
import { ActivatedRoute } from '@angular/router';
interface CobrancaTableViewModel {

  clienteId: number
  linhas: ICobranca []
}

@Component({
  selector: 'app-cobrancas',
  templateUrl: './cobrancas-table.component.html',
  styleUrls: ['./cobrancas-table.component.css']
})
export class CobrancasTableComponent implements OnInit {
  vm: CobrancaTableViewModel = {
    clienteId:0
    ,linhas: [
      { id: 0, clienteId: 0, descricao: '',dataVencimento: new Date(),valor: 0.0, pago: false }
    ]
  };
  newCobranca: ICobranca = { id: 0, clienteId: 0, descricao: '',dataVencimento: new Date(),valor: 0.0, pago: false };
  state: 'editing' | 'adding' | 'read' = 'read'
  constructor(private route: ActivatedRoute,private cobrancaService: CobrancaService) {}
  ngOnInit(): void {
    // this.route.data
    this.route.data.subscribe(d => {
      console.log('data:',d)
    })
    this.route.paramMap.subscribe(params => {
      this.vm.clienteId = +params.get('id')!
      this.loadCobrancas();
    })

  }
  clearCobranca(){
    this.newCobranca = { id: 0, clienteId: 0, descricao: '',dataVencimento: new Date(),valor: 0.0, pago: false };
    this.state = 'read'
  }
  onEditCobranca(cobranca:ICobranca){
    console.log('cobranca:',cobranca)
    this.state = 'editing'
    this.newCobranca = cobranca
  }
  closeModal(){
    this.state = 'read'
  }
  formatDate(date:Date):string {
    const d = new Date(Date.parse(date.toString()))
    return d.toLocaleDateString("pt-BR")
  }
  loadCobrancas(): void {
    this.cobrancaService.getAllCobrancas(this.vm.clienteId).subscribe(cobrancas => {

      this.vm.linhas = cobrancas;

    });
  }

  addOrUpdateCobranca(): void {
    if(this.state === 'adding'){
      this.cobrancaService.createCobranca(this.newCobranca).subscribe(cobranca => {

        this.vm.linhas.push(cobranca);

        this.clearCobranca()

      });
    }
    if(this.state === 'editing'){
      this.updateCobranca(this.newCobranca)
    }
  }


  updateCobranca(updatedCobranca: ICobranca): void {

    this.cobrancaService.updateCobranca(updatedCobranca).subscribe(cobranca => {

      const index = this.vm.linhas.findIndex(c => c.id === cobranca.id);

      if (index !== -1) {

        this.vm.linhas[index] = cobranca;

      }
      this.clearCobranca()
    });

  }


  deleteCobranca(cobrancaId: number): void {

    this.cobrancaService.deleteCobranca(cobrancaId).subscribe(() => {

      this.vm.linhas = this.vm.linhas.filter(c => c.id !== cobrancaId);

    });

  }
}
