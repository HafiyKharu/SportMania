export function LoadingSpinner({ text = 'Loading...' }: { text?: string }) {
  return (
    <div className="text-center py-12">
      <div className="inline-block w-8 h-8 border-4 border-sm-primary border-t-transparent rounded-full animate-spin" />
      <p className="text-sm-muted mt-3">{text}</p>
    </div>
  );
}
