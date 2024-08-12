import { Component, OnInit } from '@angular/core';
import { CobrancaService } from 'src/app/services/cobranca.service';
import { Cobranca } from 'src/app/services/cobranca.model';
import { ActivatedRoute } from '@angular/router';
interface CobrancaTableViewModel {

  clienteId: number  
  linhas: Cobranca []
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
  newCobranca: Cobranca = { id: 0, clienteId: 0, descricao: '',dataVencimento: new Date(),valor: 0.0, pago: false };  

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
  formatDate(date:Date):string {
    const d = new Date(Date.parse(date.toString()))        
    return d.toLocaleDateString("pt-BR")    
  }
  loadCobrancas(): void {
    this.cobrancaService.getAllCobrancas(this.vm.clienteId).subscribe(cobrancas => {

      this.vm.linhas = cobrancas;

    });
  }

  addCobranca(): void {

    this.cobrancaService.createCobranca(this.newCobranca).subscribe(cobranca => {

      this.vm.linhas.push(cobranca);

      this.newCobranca = { id: 0, clienteId: 0, valor: 0, dataVencimento: new Date(), pago: false, descricao: '' }; // Reset form

    });

  }


  updateCobranca(updatedCobranca: Cobranca): void {

    this.cobrancaService.updateCobranca(updatedCobranca).subscribe(cobranca => {

      const index = this.vm.linhas.findIndex(c => c.id === cobranca.id);

      if (index !== -1) {

        this.vm.linhas[index] = cobranca;

      }

    });

  }


  deleteCobranca(cobrancaId: number): void {

    this.cobrancaService.deleteCobranca(cobrancaId).subscribe(() => {

      this.vm.linhas = this.vm.linhas.filter(c => c.id !== cobrancaId);

    });

  }
}