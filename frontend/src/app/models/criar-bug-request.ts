export interface CriarBugRequest{
  titulo: string;
  linguagem: string;
  mensagemErro?: string | null;
  descricao: string;
  causa?: string | null;
  solucao?: string | null;
}