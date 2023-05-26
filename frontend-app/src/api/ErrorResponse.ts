export type ErrorResponse = {
  message: string;
};

export const isErrorReponse = (x: any): x is ErrorResponse => {
  return (x as ErrorResponse).message !== undefined;
}
