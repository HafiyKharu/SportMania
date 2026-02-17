'use client';

import { useState, useEffect } from 'react';
import { useRouter } from 'next/navigation';
import { planService } from '@/services/planService';
import { LoadingSpinner } from '@/components/LoadingSpinner';
import type { PlanDto, PlanDetailDto } from '@/types';

const API_BASE_URL = process.env.NEXT_PUBLIC_API_BASE_URL || 'http://localhost:5235';

type FormErrors = {
  name?: string;
  description?: string;
  price?: string;
  duration?: string;
  categoryCode?: string;
  imageUrl?: string;
  details?: string;
};

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
  const [formErrors, setFormErrors] = useState<FormErrors>({});

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
    setFormErrors({});

    try {
      const trimmedName = name.trim();
      const trimmedDescription = description.trim();
      const trimmedCategoryCode = categoryCode.trim();
      const trimmedPrice = price.trim();
      const trimmedDuration = duration.trim();
      const validDetails = details
        .map((d) => d.value.trim())
        .filter((v) => v.length > 0);

      const errors: FormErrors = {};

      if (!imageUrl) errors.imageUrl = 'Please select or upload an image.';
      if (trimmedName.length < 3) errors.name = 'Name must be at least 3 characters.';
      if (trimmedDescription.length < 10)
        errors.description = 'Description must be at least 10 characters.';
      if (trimmedCategoryCode.length === 0)
        errors.categoryCode = 'Category code is required.';
      if (!/^\d+(\.\d{1,2})?$/.test(trimmedPrice) || Number(trimmedPrice) <= 0)
        errors.price = 'Enter a valid price (e.g., 10.00).';
      if (!/^\d+$/.test(trimmedDuration) || Number(trimmedDuration) <= 0)
        errors.duration = 'Enter a valid number of days (e.g., 30).';
      if (validDetails.length === 0)
        errors.details = 'Please add at least one detail.';
      if (validDetails.some((d) => d.length < 3))
        errors.details = 'Each detail must be at least 3 characters.';

      if (Object.keys(errors).length > 0) {
        setFormErrors(errors);
        setErrorMessage('Please fix the highlighted fields.');
        setIsSubmitting(false);
        return;
      }

      const planId = crypto.randomUUID();
      const plan: PlanDto = {
        planId,
        name: trimmedName,
        description: trimmedDescription,
        price: trimmedPrice,
        duration: trimmedDuration,
        categoryCode: trimmedCategoryCode,
        imageUrl,
        details: validDetails.map((value) => ({
          planDetailsId: crypto.randomUUID(),
          planId,
          value,
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
      {isSubmitting && (
        <div className="fixed inset-0 z-50 bg-black/40 flex items-center justify-center">
          <LoadingSpinner />
        </div>
      )}
      <h1 className="text-3xl font-bold text-sm-text-light mb-6">Create New Plan</h1>

      {errorMessage && (
        <div className="bg-red-500/10 border border-red-500 text-red-400 px-4 py-3 rounded mb-4">
          {errorMessage}
        </div>
      )}

      <form onSubmit={handleSubmit} noValidate className="bg-sm-card border border-sm-border rounded-lg p-6">
        <div className="space-y-4">
          <div>
            <label className="block text-sm text-sm-muted mb-1">Name</label>
            <input
              type="text"
              required
              minLength={3}
              maxLength={100}
              value={name}
              onChange={(e) => setName(e.target.value)}
              className={`w-full px-3 py-2 bg-sm-bg border rounded text-sm-text focus:outline-none focus:border-sm-primary ${
                formErrors.name ? 'border-red-500' : 'border-sm-border'
              }`}
            />
            {formErrors.name && <p className="text-xs text-red-400 mt-1">{formErrors.name}</p>}
          </div>

          <div>
            <label className="block text-sm text-sm-muted mb-1">Description</label>
            <textarea
              required
              minLength={10}
              maxLength={1000}
              value={description}
              onChange={(e) => setDescription(e.target.value)}
              className={`w-full px-3 py-2 bg-sm-bg border rounded text-sm-text focus:outline-none focus:border-sm-primary ${
                formErrors.description ? 'border-red-500' : 'border-sm-border'
              }`}
              rows={3}
            />
            {formErrors.description && (
              <p className="text-xs text-red-400 mt-1">{formErrors.description}</p>
            )}
          </div>

          <div className="grid grid-cols-2 gap-4">
            <div>
              <label className="block text-sm text-sm-muted mb-1">Price (RM)</label>
              <input
                type="text"
                required
                inputMode="decimal"
                pattern="^\d+(\.\d{1,2})?$"
                value={price}
                onChange={(e) => setPrice(e.target.value)}
                className={`w-full px-3 py-2 bg-sm-bg border rounded text-sm-text focus:outline-none focus:border-sm-primary ${
                  formErrors.price ? 'border-red-500' : 'border-sm-border'
                }`}
              />
              {formErrors.price && <p className="text-xs text-red-400 mt-1">{formErrors.price}</p>}
              <p className="text-xs text-sm-muted mt-1">Example: 10.00</p>
            </div>
            <div>
              <label className="block text-sm text-sm-muted mb-1">Duration (days)</label>
              <input
                type="text"
                required
                inputMode="numeric"
                pattern="^\d+$"
                value={duration}
                onChange={(e) => setDuration(e.target.value)}
                className={`w-full px-3 py-2 bg-sm-bg border rounded text-sm-text focus:outline-none focus:border-sm-primary ${
                  formErrors.duration ? 'border-red-500' : 'border-sm-border'
                }`}
              />
              {formErrors.duration && (
                <p className="text-xs text-red-400 mt-1">{formErrors.duration}</p>
              )}
            </div>
          </div>

          <div>
            <label className="block text-sm text-sm-muted mb-1">Category Code</label>
            <input
              type="text"
              value={categoryCode}
              required
              minLength={1}
              maxLength={50}
              onChange={(e) => setCategoryCode(e.target.value)}
              placeholder="Enter category code"
              className={`w-full px-3 py-2 bg-sm-bg border rounded text-sm-text focus:outline-none focus:border-sm-primary ${
                formErrors.categoryCode ? 'border-red-500' : 'border-sm-border'
              }`}
            />
            {formErrors.categoryCode && (
              <p className="text-xs text-red-400 mt-1">{formErrors.categoryCode}</p>
            )}
          </div>

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
                  required={index === 0}
                  minLength={3}
                  maxLength={200}
                  onChange={(e) => updateDetail(index, e.target.value)}
                  className={`flex-1 px-3 py-2 bg-sm-bg border rounded text-sm-text focus:outline-none focus:border-sm-primary ${
                    formErrors.details ? 'border-red-500' : 'border-sm-border'
                  }`}
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
            {formErrors.details && <p className="text-xs text-red-400 mt-1">{formErrors.details}</p>}
          </div>

          <div>
            <label className="block text-sm text-sm-muted mb-1">Image</label>
            <select
              value={imageUrl}
              onChange={(e) => setImageUrl(e.target.value)}
              className={`w-full px-3 py-2 bg-sm-bg border rounded text-sm-text focus:outline-none focus:border-sm-primary mb-2 ${
                formErrors.imageUrl ? 'border-red-500' : 'border-sm-border'
              }`}
            >
              <option value="">Select an image...</option>
              {mediaPaths.map((path) => (
                <option key={path} value={path}>
                  {path.split('/').pop()}
                </option>
              ))}
            </select>
            {formErrors.imageUrl && <p className="text-xs text-red-400 mb-2">{formErrors.imageUrl}</p>}

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
