import { ICobranca } from "./cobranca.model";

export interface Cliente {

    id: number;
    
    usuarioId: number;

    documento: string;
  
    nome: string;

    telefone: string;
    
    endereco: string;
    cobrancas: ICobranca []
}
export interface ClienteRowViewModel {
    id: number
    nome: string
    pagos: number
    abertos: number
    atrasados: number
}
export function getSituacaoPagamento(cobranca:ICobranca){
    if(cobranca.pago){
        return 'pago'
    }
    const d = new Date(Date.parse(cobranca.dataVencimento.toString()))
    if(cobranca.pago && new Date() < d){
        return 'aberto'
    }
    return 'atrasado'
}
//Mapeando para um modelo mais apropriado para a view
export function toView(cliente:Cliente): ClienteRowViewModel {
    const situacoes = cliente.cobrancas.map(c => getSituacaoPagamento(c))
    const pagos = situacoes.filter(situacao => situacao == 'pago').length
    const abertos = situacoes.filter(situacao => situacao == 'aberto').length
    const atrasados = situacoes.filter(situacao => situacao == 'atrasado').length
    return {
        id:cliente.id
        ,nome:cliente.nome
        ,pagos
        ,abertos
        ,atrasados
    }
}  