export function LoadingSpinner({ text = 'Loading...' }: { text?: string }) {
  return (
    <div className="flex flex-col items-center justify-center min-h-[200px] w-full animate-fade-in">
      <div className="relative w-16 h-16 mb-4">
        {/* Outer pulsating ring */}
        <div className="absolute inset-0 rounded-full border-4 border-sm-primary opacity-25 animate-ping"></div>
        
        {/* Inner spinning ring */}
        <div className="absolute inset-0 rounded-full border-4 border-t-sm-primary border-r-transparent border-b-sm-primary border-l-transparent animate-spin"></div>
        
        {/* Center dot (optional) */}
        <div className="absolute inset-0 m-auto w-2 h-2 bg-sm-primary rounded-full"></div>
      </div>
      <p className="text-sm-muted font-medium tracking-wide animate-pulse">{text}</p>
    </div>
  );
}
