export interface BugResponse{
  id: number;
  titulo: string;
  linguagem: string;
  mensagemErro?: string | null;
  descricao: string;
  causa?: string | null;
  solucao?: string | null;
  status: string;
  dataCriacao: string;
  dataAtualizacao?: string | null;
}