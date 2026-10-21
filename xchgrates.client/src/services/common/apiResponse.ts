export class ApiResponse<Data> {
  success: boolean = false;
  result: Data | null = null;
  error: string | null = null;
}
