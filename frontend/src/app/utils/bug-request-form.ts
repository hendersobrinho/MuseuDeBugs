export interface BugRequestForm {
  titulo: string;
  linguagem: string;
  descricao: string;
}

export function validateBugRequestForm(request: BugRequestForm): string | null {
  if (request.titulo.length < 1) {
    return 'Informe o titulo.';
  }

  if (request.linguagem.length < 1) {
    return 'Informe a linguagem. C# e aceito.';
  }

  if (request.descricao.length < 10) {
    return 'Informe uma descricao com pelo menos 10 caracteres.';
  }

  return null;
}

export function normalizeBugLanguage(value: string): string {
  const normalized = value.trim();
  const knownLanguages: Record<string, string> = {
    'c#': 'C#',
    'csharp': 'C#',
    'c-sharp': 'C#',
    'c++': 'C++',
    'cpp': 'C++',
    'js': 'JavaScript',
    'javascript': 'JavaScript',
    'ts': 'TypeScript',
    'typescript': 'TypeScript'
  };

  return knownLanguages[normalized.toLowerCase()] ?? normalized;
}

export function normalizeOptionalBugText(value: string | null | undefined): string | null {
  const normalized = value?.trim() ?? '';
  return normalized.length > 0 ? normalized : null;
}
