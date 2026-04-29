import { BugCard } from '../models/bug-card';

export const BUGS_MOCK: BugCard[] = [
  {
    id: 1,
    title: 'NullReferenceException ao abrir detalhe',
    language: 'C#',
    status: 'Resolvido',
    errorMessage: 'System.NullReferenceException: Object reference not set...',
    cause: 'Objeto era usado antes de ser inicializado no service.',
    updatedAt: 'Atualizado ha 2h',
    signal: 'learned'
  },
  {
    id: 3,
    title: 'Cookie nao enviado pelo navegador',
    language: 'Angular',
    status: 'Aberto',
    errorMessage: '401 Unauthorized mesmo apos login admin',
    cause: 'Chamada HTTP ainda estava sem withCredentials.',
    updatedAt: 'Atualizado ha 3 dias',
    signal: 'info'
  },
];
