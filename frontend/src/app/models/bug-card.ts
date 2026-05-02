export type BugStatus = 'Aberto' | 'Resolvido';

export type BugSignal = 'learned' | 'warning' | 'info' | 'danger';

export interface BugCard {
  id: number;
  title: string;
  language: string;
  status: BugStatus;
  errorMessage: string;
  description: string;
  cause: string;
  solution: string;
  createdAt: string;
  updatedAt: string;
  signal: BugSignal;
}
