import { useEffect, useState } from "react";
import { subscriptionService } from "../Services/Subscription/subscriptionService";

const Subscription = () => {
  const [subscriptions, setSubscriptions] = useState<any[]>([]);
  const [loading, setLoading] = useState(false);

  const loadSubscriptions = async () => {
    try {
      setLoading(true);

      const data = await subscriptionService.userSubscriptions();

      setSubscriptions(data);
    } catch (error) {
      console.error(error);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadSubscriptions();
  }, []);

  const handlePause = async (id: string) => {
    await subscriptionService.pauseSubscription(id);
    await loadSubscriptions();
  };

  const handleActivate = async (id: string) => {
    await subscriptionService.activateSubscription(id);
    await loadSubscriptions();
  };

  const handleCancel = async (id: string) => {
    await subscriptionService.cancelSubscription(id);
    await loadSubscriptions();
  };

  if (loading) {
    return <h2>Loading...</h2>;
  }

  return (
    <div className="p-6">
      <div className="flex justify-between mb-6">
        <h1 className="text-3xl font-bold">
          Subscriptions
        </h1>

        <button className="bg-blue-600 text-white px-4 py-2 rounded-lg">
          + Add Subscription
        </button>
      </div>

      <div className="grid md:grid-cols-2 lg:grid-cols-3 gap-4">
        {subscriptions.map((subscription) => (
          <div
            key={subscription.id}
            className="bg-white p-5 rounded-xl shadow"
          >
            <h2 className="text-xl font-semibold">
              {subscription.name}
            </h2>

            <p>₹ {subscription.amount}</p>

            <p>
              {subscription.billingCycle}
            </p>

            <p>
              Next Billing:
              {" "}
              {new Date(
                subscription.nextBillingDate
              ).toLocaleDateString()}
            </p>

            <p className="mt-2 font-medium">
              Status: {subscription.status}
            </p>

            <div className="flex gap-2 mt-4 flex-wrap">
              {subscription.status === "Active" && (
                <button
                  onClick={() =>
                    handlePause(subscription.id)
                  }
                  className="bg-yellow-500 text-white px-3 py-1 rounded"
                >
                  Pause
                </button>
              )}

              {subscription.status === "Paused" && (
                <button
                  onClick={() =>
                    handleActivate(subscription.id)
                  }
                  className="bg-green-600 text-white px-3 py-1 rounded"
                >
                  Resume
                </button>
              )}

              <button
                onClick={() =>
                  handleCancel(subscription.id)
                }
                className="bg-red-600 text-white px-3 py-1 rounded"
              >
                Cancel
              </button>

              <button
                className="bg-blue-600 text-white px-3 py-1 rounded"
              >
                Edit
              </button>
            </div>
          </div>
        ))}
      </div>
    </div>
  );
};

export default Subscription;