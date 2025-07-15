package com.rateus;

import android.app.Activity;
import android.util.Log;

import com.google.android.play.core.review.ReviewManager;
import com.google.android.play.core.review.ReviewManagerFactory;
import com.google.android.play.core.tasks.Task;
import com.google.android.play.core.review.ReviewInfo;

public class RateUs {
    private static final String TAG = "RateUs";

    public static void requestReview(Activity activity) {
        ReviewManager manager = ReviewManagerFactory.create(activity);
        Task<ReviewInfo> request = manager.requestReviewFlow();

        request.addOnCompleteListener(task -> {
            if (task.isSuccessful()) {
                ReviewInfo reviewInfo = task.getResult();
                Task<Void> flow = manager.launchReviewFlow(activity, reviewInfo);
                flow.addOnCompleteListener(finish -> Log.d(TAG, "In-app review flow finished."));
            } else {
                Log.e(TAG, "Review flow request failed.");
            }
        });
    }
}
