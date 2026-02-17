'use client';

import { useState, useEffect } from 'react';
import { useRouter } from 'next/navigation';
import { planService } from '@/services/planService';
import { LoadingSpinner } from '@/components/LoadingSpinner';
import type { PlanDto, PlanDetailDto } from '@/types';

const API_BASE_URL = process.env.NEXT_PUBLIC_API_BASE_URL || 'http://localhost:5235';

export default function PlanCreatePage() {
  const router = useRouter();
  const [name, setName] = useState('');
  const [description, setDescription] = useState('');
  const [price, setPrice] = useState('');
  const [duration, setDuration] = useState('');
  const [categoryCode, setCategoryCode] = useState('');
  const [imageUrl, setImageUrl] = useState('');
  const [details, setDetails] = useState<{ value: string }[]>([{ value: '' }]);
  const [mediaPaths, setMediaPaths] = useState<string[]>([]);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [errorMessage, setErrorMessage] = useState('');
  const [isUploading, setIsUploading] = useState(false);
  const [uploadMessage, setUploadMessage] = useState('');

  useEffect(() => {
    loadMedia();
  }, []);

  async function loadMedia() {
    const paths = await planService.getMediaPaths();
    setMediaPaths(paths);
  }

  function addDetail() {
    setDetails([...details, { value: '' }]);
  }

  function removeDetail(index: number) {
    setDetails(details.filter((_, i) => i !== index));
  }

  function updateDetail(index: number, value: string) {
    const updated = [...details];
    updated[index] = { value };
    setDetails(updated);
  }

  async function handleFileUpload(e: React.ChangeEvent<HTMLInputElement>) {
    const file = e.target.files?.[0];
    if (!file) return;

    const allowedTypes = ['image/jpeg', 'image/png', 'image/gif', 'image/webp'];
    if (!allowedTypes.includes(file.type)) {
      setUploadMessage('Invalid file type. Only JPG, PNG, GIF, and WebP are allowed.');
      return;
    }

    if (file.size > 5 * 1024 * 1024) {
      setUploadMessage('File too large. Maximum size is 5MB.');
      return;
    }

    setIsUploading(true);
    setUploadMessage('');

    const path = await planService.uploadImage(file);
    if (path) {
      setImageUrl(path);
      setMediaPaths((prev) => [...prev, path]);
      setUploadMessage('Image uploaded successfully!');
    } else {
      setUploadMessage('Failed to upload image.');
    }
    setIsUploading(false);
  }

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    setIsSubmitting(true);
    setErrorMessage('');

    try {
      if (!imageUrl) {
        setErrorMessage('Please select or upload an image.');
        setIsSubmitting(false);
        return;
      }

      const planId = crypto.randomUUID();
      const plan: PlanDto = {
        planId,
        name,
        description,
        price,
        duration,
        categoryCode,
        imageUrl,
        details: details
          .filter((d) => d.value.trim())
          .map((d) => ({
            planDetailsId: crypto.randomUUID(),
            planId,
            value: d.value,
          })),
      };

      await planService.createPlan(plan);
      router.push('/plans');
    } catch {
      setErrorMessage('Failed to create plan. Please try again.');
    } finally {
      setIsSubmitting(false);
    }
  }

  return (
    <div className="max-w-3xl mx-auto">
      <h1 className="text-3xl font-bold text-sm-text-light mb-6">Create New Plan</h1>

      {errorMessage && (
        <div className="bg-red-500/10 border border-red-500 text-red-400 px-4 py-3 rounded mb-4">
          {errorMessage}
        </div>
      )}

      <form onSubmit={handleSubmit} className="bg-sm-card border border-sm-border rounded-lg p-6">
        <div className="space-y-4">
          <div>
            <label className="block text-sm text-sm-muted mb-1">Name</label>
            <input
              type="text"
              required
              value={name}
              onChange={(e) => setName(e.target.value)}
              className="w-full px-3 py-2 bg-sm-bg border border-sm-border rounded text-sm-text focus:outline-none focus:border-sm-primary"
            />
          </div>

          <div>
            <label className="block text-sm text-sm-muted mb-1">Description</label>
            <textarea
              required
              value={description}
              onChange={(e) => setDescription(e.target.value)}
              className="w-full px-3 py-2 bg-sm-bg border border-sm-border rounded text-sm-text focus:outline-none focus:border-sm-primary"
              rows={3}
            />
          </div>

          <div className="grid grid-cols-2 gap-4">
            <div>
              <label className="block text-sm text-sm-muted mb-1">Price (RM)</label>
              <input
                type="text"
                required
                value={price}
                onChange={(e) => setPrice(e.target.value)}
                className="w-full px-3 py-2 bg-sm-bg border border-sm-border rounded text-sm-text focus:outline-none focus:border-sm-primary"
              />
            </div>
            <div>
              <label className="block text-sm text-sm-muted mb-1">Duration (days)</label>
              <input
                type="text"
                required
                value={duration}
                onChange={(e) => setDuration(e.target.value)}
                className="w-full px-3 py-2 bg-sm-bg border border-sm-border rounded text-sm-text focus:outline-none focus:border-sm-primary"
              />
            </div>
          </div>

          <div>
            <label className="block text-sm text-sm-muted mb-1">Category Code</label>
            <input
              type="text"
              value={categoryCode}
              required
              onChange={(e) => setCategoryCode(e.target.value)}
              placeholder="e.g. TP-ABC123"
              className="w-full px-3 py-2 bg-sm-bg border border-sm-border rounded text-sm-text focus:outline-none focus:border-sm-primary"
            />
          </div>

          {/* Details */}
          <div>
            <div className="flex justify-between items-center mb-2">
              <label className="text-sm text-sm-muted">Plan Details</label>
              <button
                type="button"
                onClick={addDetail}
                className="text-sm text-sm-primary hover:text-blue-400"
              >
                + Add Detail
              </button>
            </div>
            {details.map((detail, index) => (
              <div key={index} className="flex gap-2 mb-2">
                <input
                  type="text"
                  value={detail.value}
                  required
                  onChange={(e) => updateDetail(index, e.target.value)}
                  className="flex-1 px-3 py-2 bg-sm-bg border border-sm-border rounded text-sm-text focus:outline-none focus:border-sm-primary"
                  placeholder={`Detail ${index + 1}`}
                />
                {details.length > 1 && (
                  <button
                    type="button"
                    onClick={() => removeDetail(index)}
                    className="px-3 py-2 bg-red-600 text-white rounded hover:bg-red-700 text-sm"
                  >
                    Remove
                  </button>
                )}
              </div>
            ))}
          </div>

          {/* Image Section */}
          <div>
            <label className="block text-sm text-sm-muted mb-1">Image</label>

            <select
              value={imageUrl}
              onChange={(e) => setImageUrl(e.target.value)}
              className="w-full px-3 py-2 bg-sm-bg border border-sm-border rounded text-sm-text focus:outline-none focus:border-sm-primary mb-2"
            >
              <option value="">Select an image...</option>
              {mediaPaths.map((path) => (
                <option key={path} value={path}>
                  {path.split('/').pop()}
                </option>
              ))}
            </select>

            <div className="flex items-center gap-3">
              <label className="cursor-pointer px-3 py-2 bg-sm-btn-sec border border-sm-btn-sec-border rounded text-sm text-sm-text-light hover:bg-sm-hover">
                Upload New Image
                <input
                  type="file"
                  accept="image/jpeg,image/png,image/gif,image/webp"
                  onChange={handleFileUpload}
                  className="hidden"
                />
              </label>
              {isUploading && (
                <span className="inline-block w-4 h-4 border-2 border-sm-primary border-t-transparent rounded-full animate-spin" />
              )}
            </div>

            {uploadMessage && (
              <p className={`text-sm mt-1 ${uploadMessage.includes('success') ? 'text-green-400' : 'text-red-400'}`}>
                {uploadMessage}
              </p>
            )}

            {imageUrl && (
              <div className="mt-3">
                <img
                  src={`${API_BASE_URL}${imageUrl}`}
                  alt="Preview"
                  className="max-h-48 rounded border border-sm-border"
                />
              </div>
            )}
          </div>
        </div>

        <div className="flex gap-3 mt-6">
          <button
            type="button"
            onClick={() => router.push('/plans')}
            className="px-4 py-2 bg-sm-btn-sec border border-sm-btn-sec-border rounded text-sm-text-light hover:bg-sm-hover"
          >
            Cancel
          </button>
          <button
            type="submit"
            disabled={isSubmitting}
            className="px-4 py-2 bg-sm-primary text-white rounded hover:bg-blue-600 disabled:opacity-50"
          >
            {isSubmitting ? 'Creating...' : 'Create Plan'}
          </button>
        </div>
      </form>
    </div>
  );
}
