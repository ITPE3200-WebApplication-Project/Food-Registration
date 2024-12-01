interface DeleteModalProps {
  show: boolean;
  productId: number | null;
  productName: string;
  onClose: () => void;
  onConfirm: (id: number) => void;
}

export default function DeleteModal({
  show,
  productId,
  productName,
  onClose,
  onConfirm,
}: DeleteModalProps) {
  if (!show || !productId) return null;

  return (
    <div
      className="modal fade show"
      style={{ display: "block" }}
      id="deleteModal"
      tabIndex={-1}
      aria-labelledby="deleteModalLabel"
      aria-hidden="true"
    >
      <div className="modal-dialog">
        <div className="modal-content">
          <div className="modal-header">
            <h5 className="modal-title text-danger" id="deleteModalLabel">
              Confirm Deletion
            </h5>
            <button
              type="button"
              className="btn-close"
              onClick={onClose}
              aria-label="Close"
            />
          </div>
          <div className="modal-body">
            Are you sure you want to delete the product "
            <strong>{productName}</strong>"? This action cannot be undone.
          </div>
          <div className="modal-footer">
            <button
              onClick={() => onConfirm(productId)}
              className="btn btn-danger"
            >
              Yes, Delete
            </button>
            <button onClick={onClose} className="btn btn-secondary">
              Cancel
            </button>
          </div>
        </div>
      </div>
    </div>
  );
}
