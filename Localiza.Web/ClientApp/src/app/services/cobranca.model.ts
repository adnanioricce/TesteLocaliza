
export interface ICobranca {
    id: number
    clienteId: number
    valor: number
    dataVencimento: Date
    pago: boolean
    descricao: string    
}
